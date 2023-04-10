using System;
using System.Collections.Generic;
using DtTelloDrone.Logger;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.Services;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Models;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using NLog;
using NLog.Fluent;

namespace DtTelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter
{
    #region Properties and Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    
    private LandScapeLayer _layer;

    private ICore _core;
    private StateDeterminer _stateDeterminer;

    private TelloStateParameter _prevParameters;
    
    private DroneState _currentDroneState;
    private DroneState _prevDroneState;

    private int _travelDurationCounter = 0;

    private int _tickCount = 0;
    private Random _random = new();

    private DateTime _lastUpdateTS;
    
    // --------- digital Entity information
    
    /// <summary>
    /// The unique identifier of the agent.
    /// </summary>
    public Guid ID { get; set; }

    public Position Position { get; set; }
    
    public Position Target { get; set; }
    
    
    private List<Position> _directions;
    private byte _speed;

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
    
    private const int DefaultSpeed = 30;
    private const int DefaultBearing = 0;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        _core = TelloCore.GetInstance();
        
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
            
        _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
        _speed = DefaultSpeed;
        Bearing = DefaultBearing;
        Logger.Trace(_layer);
        Logger.Info("Agent has been initialized");
    }

    #endregion

    #region Tick

    public void Tick()
    {
        // Lese Zustandsparameter aus
        var parameters = _core.GetStateParameter();

        if (parameters == null)
        {
            return;
        }

        // Bestimme den Zustand der Tello Drohne
        if (parameters.TimeStamp == _lastUpdateTS)
        {
            return;
        }

        DroneState state = _stateDeterminer.DetermineState(parameters);
        
        Logger.Trace($"Current Drone state is: {state.ToString()}");
        if (state != _prevDroneState)
        {
            Logger.Info($"Drone is {state}");
        }

        // Zustand in die Simulation überführen
        //MapParameters(state, parameters);
        _prevParameters = parameters;
        _prevDroneState = state;
        _lastUpdateTS = parameters.TimeStamp;

        // Entscheidungsfindungs
        _tickCount++;
    }

    #endregion
    
    #region Private Methods

    private void MapParameters(DroneState currentState, TelloStateParameter parameters)
    {
        if (_prevParameters == null)
        {
            return;
        }
        
        double timeInterval = parameters.TimeStamp.Second - _prevParameters.TimeStamp.Second;

        // Calculate bearing
        Bearing = DataMapper.MapToMarsBearing(parameters.Yaw);
        //Logger.Trace(Bearing);
        double motionBearing = DataMapper.GetMotionBearing(currentState);
        double flyDirection = (Bearing + motionBearing) % 360;

        // calculate travelled distance
        double acceleration = parameters.AccelerationX;       // cm/s
        double velocity = parameters.VelocityX * 10;          // cm/s

        /*
        if (_currentDroneState == DroneState.MovingForwards || _currentDroneState == DroneState.MovingBackwards)
        {
            acceleration = parameters.AccelerationX;
            velocity = parameters.VelocityX;
        }
        else if (_currentDroneState == DroneState.MovingRight || _currentDroneState == DroneState.MovingLeft)
        {
            acceleration = parameters.AccelerationY;
            velocity = parameters.VelocityY;
        }
        */
        
        double travelledDistance = DataMapper.CalculateTravelledDistance(timeInterval, acceleration, velocity);
        Logger.Trace($"Travelled Distance: {travelledDistance}");

        // move the agent to the corresponding position of the drone.
        Position = _layer._landScapeEnvironment.Move(this, flyDirection, travelledDistance); // nicht geprüft
        Logger.Trace($"Agent moved to {Position}");
        _height = parameters.Height;
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
    
            
    /// <summary>
    /// Check if the selected action is a movement.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private bool IsMovementAction(TelloAction action)
    {
        // TODO: In eine static Methode auslagern.
        return
            action == TelloAction.MoveForward ||
            action == TelloAction.MoveBackward ||
            action == TelloAction.MoveLeft ||
            action == TelloAction.MoveRight ||
            action == TelloAction.Rise ||
            action == TelloAction.Sink;
    }

    private bool CheckObstacleCollision()
    {
        return false;
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