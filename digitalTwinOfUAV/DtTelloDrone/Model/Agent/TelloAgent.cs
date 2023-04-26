using System;
using System.Collections.Generic;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.PathPlanning;
using DtTelloDrone.Model.Services;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;
using DtTelloDrone.Shared;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NLog;

namespace DtTelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter, IPositionable, ICoreSubscriber
{
    #region Properties and Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    private static readonly ResourceDirectoryManager ResourceManager = ResourceDirectoryManager.GetDirectoryManager();
    
    private readonly ICore _core = TelloCore.GetInstance();
    private readonly StateDeterminer _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
    private Random _random = new();

    private LandScapeLayer _layer;
    private int _tickCount = 0;

    private CheckpointNavigation _checkpointNavigation = new();
    private Queue<CoreMessage> _messages = new();
    private CoreMessage _message = new();
    
    private Operation _operation = Operation.None;
    private PlaybackNavigation _playbackNavigation;
    
    private PlaybackNavigationRecord _nextRecord;
    private DateTime _lastExecActionTs = DateTime.Now;
    private long _waitTime;
    
    private Position _target = null;

    public Guid ID { get; set; }
    private int _height;
    public Position Position { get; set; }
    private TelloStateParameter _prevParameters;
    private DroneState _prevDroneState;
    private DateTime _lastUpdateTs;

    #endregion

    #region Initialization Values

    [PropertyDescription (Name ="StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription (Name = "StartY")]
    public int StartY { get; set; }
    
    [PropertyDescription (Name = "Bearing")]
    public double Bearing { get; set; }
    
    [PropertyDescription (Name ="Speed")]
    public int Speed { get; set; }
    
    #endregion

    #region Constants

    private const double DistanceTolerance = 5;
    private const double BearingTolerance = 2;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
        _playbackNavigation = new PlaybackNavigation();
        _core.Subscribe(this);
        
        Logger.Trace(_layer);
        Logger.Info("Agent has been initialized");
    }

    #endregion

    #region Tick

    public void Tick()
    {
        try
        {
            if (_messages.TryDequeue(out _message))                 
                ReadMessage(_message);
            
            // Aktuellen Zustandsparameter auslesen.
            var parameters = _core.GetStateParameter();

            if (parameters == null) 
                return;

            if (parameters.TimeStamp == _lastUpdateTs) 
                return;

            DroneState state = _stateDeterminer.DetermineState(parameters);
            
            if (state != _prevDroneState) 
                Logger.Trace($"Drone changed to {state}");

            // Agentenparameter aktualisieren
            UpdateAgentState(parameters, state);

            // Aktion ausführen
            _prevParameters = parameters;
            _prevDroneState = state;
            _lastUpdateTs = parameters.TimeStamp;

            ExecuteNextOperation();
            _tickCount++;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            DroneCommand command = new DroneCommand(TelloAction.Emergency, 0);
            _core.QueryCommand(command);
        }
    }
    
    #endregion

    #region Public Methods

    public void PublishMessage(CoreMessage message)
    {
        _messages.Enqueue(message);
    }

    #endregion
    
    #region Private Methods

    private void UpdateAgentState(TelloStateParameter parameters, DroneState state)
    {
        if (_prevParameters == null)
        {
            return;
        }

        Bearing = DataMapper.MapToMarsBearing(parameters.Yaw);

        if (state == DroneState.MovingForwards || 
            state == DroneState.MovingBackwards ||
            state == DroneState.MovingLeft || 
            state == DroneState.MovingRight ||
            state == DroneState.Unknown)
        {
            Position = UpdatePosition(parameters);
        }
        
        _height = parameters.Height;

        Logger.Trace($"Agent moved to {Position}");
    }

    /// <summary>
    /// Update the position depending on the new parameters.
    /// </summary>
    /// <param name="state">The current drone state.</param>
    /// <param name="parameters">The new parameters</param>
    /// <returns>The new position the drone is moving to.</returns>
    private Position UpdatePosition(TelloStateParameter parameters)
    {
        double timeInterval = Math.Abs(parameters.TimeStamp.Millisecond - _prevParameters.TimeStamp.Millisecond);
        timeInterval /= 1000;
        
        double accelerationX = Math.Round(parameters.AccelerationX) * -1;       // cm/ms^2
        double accelerationY = Math.Round(parameters.AccelerationY) * -1;
        double velocityX = parameters.VelocityX;             // cm/ms
        double velocityY = parameters.VelocityY;

        if (TelloFlightMetrics.ForwardAccelerationThreshold < accelerationX &&
            accelerationX < TelloFlightMetrics.BackwardAccelerationThreshold)
        {
            accelerationX = 0;
        }
        
        if (TelloFlightMetrics.LeftAccelerationThreshold < accelerationY &&
            accelerationY < TelloFlightMetrics.RightAccelerationThreshold)
        {
            accelerationY = 0;
        }
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        Vector<double> vec1 = new DenseVector(new []{speedX, 0});
        Vector<double> vec2 = new DenseVector(new []{0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        double motionBearingMars = DataMapper.MapNormalCoordinateToMars(motionBearing);

        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double travelingDistance = (DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2)) / 10;

        Logger.Trace($"Bearing {Bearing}");
        
        if (travelingDistance >= 0.1)
        {
            Logger.Trace($"FlyDirection {flyDirection}");
            Logger.Trace($"TravelingDistance {travelingDistance}");
            Position = _layer._landScapeEnvironment.Move(this, flyDirection, travelingDistance);
        }

        return Position;
    }
        

    private void ExecuteNextOperation()
    {
        if (_operation == Operation.RecordedNavigation)
        {
            if (_nextRecord == null)
            {
                _nextRecord = _playbackNavigation.GetNextRecord();
                
                if (_nextRecord == null)
                {
                    _operation = Operation.None;
                    Logger.Info("Playback Navigation beendet");
                    return;
                }
            }
            
            TelloAction action = _nextRecord._action;

            var timePastSinceLastAction = DateTime.Now - _lastExecActionTs;
            long waitTime = _playbackNavigation.GetWaitTime();
            
            Console.WriteLine(timePastSinceLastAction.TotalMilliseconds);
            
            if (action != TelloAction.NoAction && waitTime < timePastSinceLastAction.TotalMilliseconds)
            {
                DroneCommand command = new DroneCommand(action, Speed);
                _core.QueryCommand(command);
                
                _playbackNavigation.RecordExecuted();
                _nextRecord = null;
                _lastExecActionTs = DateTime.Now;
            }
        }
    }

    private bool CheckObstacleCollision()
    {
        return false;
    }

    private void ReadMessage(CoreMessage msg)
    {
        TelloAction action = msg.GetAction();

        switch (action) 
        {
            case TelloAction.SetCheckpoint: _checkpointNavigation.AddCheckpoint(Position);
                break;
            case TelloAction.DeleteCheckpoint: _checkpointNavigation.RemoveLastCheckpoint();
                break;
            case TelloAction.StartRecordedNavigation:
                _operation = Operation.RecordedNavigation; break;
            default: break;
        }
    }
    
    private void createGPX()
    {
        // Nachkommastellen reduzieren.
        // Qgis
    }
    
    #endregion

    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
    
    public CollisionKind? HandleCollision(ICharacter other)
    {
        return CollisionKind.Block;
    }

    public double Extent { get; set; }
    }