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
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter, IPositionable, ICoreSubscriber
{
    #region Properties and Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    
    private LandScapeLayer _layer;

    private readonly ICore _core = TelloCore.GetInstance();
    private readonly StateDeterminer _stateDeterminer = StateDeterminer.getStateDeterminerInstance();

    private TelloStateParameter _prevParameters;
    
    private DroneState _prevDroneState;

    private CheckpointNavigation _checkpointNavigation = new CheckpointNavigation();
    
    private int _tickCount = 0;
    private Random _random = new();

    private DateTime _lastUpdateTS;

    private Queue<CoreMessage> _messages;

    // --------- digital Entity information
    
    /// <summary>
    /// The unique identifier of the agent.
    /// </summary>
    public Guid ID { get; set; }

    public Position Position { get; set; }
    
    public Position Target { get; set; }
    
    private List<Position> _directions;
    private byte _speed = DefaultSpeed;

    /// <summary>
    /// the height measured from the starting Point
    /// </summary>
    private int _height;

    #endregion

    #region Initialization Values

    [PropertyDescription (Name ="StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription (Name = "StartY")]
    public int StartY { get; set; }
    
    [PropertyDescription (Name = "Bearing")]
    public double Bearing { get; set; }

    #endregion
    
    #region Constants
    
    private const int DefaultSpeed = 5;
    private const int DefaultBearing = 0;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);

        Logger.Trace(_layer);
        Logger.Info("Agent has been initialized");
    }

    #endregion

    #region Tick

    public void Tick()
    {
        try
        {
            // Aktuellen Zustandsparameter auslesen.
            var parameters = _core.GetStateParameter();

            // Überprüfe, ob die Daten richtig sind und wandel sie um.

            if (parameters == null)
            {
                return;
            }

            if (parameters.TimeStamp == _lastUpdateTS)
            {
                return;
            }

            // Zustand der Drohne bestimmen

            DroneState state = _stateDeterminer.DetermineState(parameters);

            //Logger.Trace($"Current Drone state is: {state.ToString()}");
            if (state != _prevDroneState)
            {
                Logger.Info($"Drone changed to {state}");
            }

            // Agent synchronisieren

            UpdateAgentState(parameters, state);

            // Aktion ausführen

            _prevParameters = parameters;
            _prevDroneState = state;
            _lastUpdateTS = parameters.TimeStamp;
            
            /*
            if (_messages.TryDequeue(out CoreMessage message))
            {
                ReadMessage(message);
            }
            */

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
        Logger.Trace($"FlyDirection {flyDirection}");
        Logger.Trace($"TravelingDistance {travelingDistance}");
        
        if (travelingDistance >= 0.1)
        {
            Position = _layer._landScapeEnvironment.Move(this, flyDirection, travelingDistance);
        }

        return Position;
    }
        

    private void DoAction()
    {
        // Check Command validity
        /*
        if (selectedAction != TelloAction.Unknown)
        {
            DroneCommand command = new DroneCommand(selectedAction, _speed);

            if (IsMovementAction(selectedAction))
            {
                if (!CheckObstacleCollision())
                {
                    _core.QueryCommand(command);
                }
            }
            else
            {
                _core.QueryCommand(command);
            }
        }*/
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
            case TelloAction.StartAutonomousNavigation: break;
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