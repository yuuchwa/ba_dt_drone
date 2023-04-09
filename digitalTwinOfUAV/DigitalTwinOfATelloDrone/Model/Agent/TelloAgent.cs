using System;
using System.Collections.Generic;
using DigitalTwinOfATelloDrone.Logger;
using DigitalTwinOfATelloDrone.Model.Attributes;
using DigitalTwinOfATelloDrone.Model.Layer;
using DigitalTwinOfATelloDrone.Model.Services;
using DigitalTwinOfATelloDrone.RyzeSDK;
using DigitalTwinOfATelloDrone.RyzeSDK.Attribute;
using DigitalTwinOfATelloDrone.RyzeSDK.Models;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;

namespace DigitalTwinOfATelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter
{
    #region Properties and Fields

    private ILogger Logger;
    
    private LandScapeLayer _layer;

    private ICore _core;
    private StateDeterminer _stateDeterminer;

    private TelloStateParameter _prevParameters;
    
    private DroneState _currentDroneState;
    private DroneState _prevDroneState;

    private double _travelDuration;
    private int _travelDurationCounter = 0;

    private int _tickCount = 0;
    private Random _random = new();
    
    // --------- digital Entity information
    
    /// <summary>
    /// The unique identifier of the agent.
    /// </summary>
    public Guid ID { get; set; }

    public Position Position { get; set; }
    
    public Position Target { get; set; }
    
    private List<Position> _directions;
    private double _bearing;
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

    #endregion

    #region Constants
    
    private const int DefaultSpeed = 30;
    private const int DefaultBearing = 0;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        Logger = new NLogLogger(GetType().Name);

        _layer = layer;
        _core = TelloCore.GetInstance();
        
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
            
        _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
        _speed = DefaultSpeed;
        _bearing = DefaultBearing;

        LogEntry entry = new LogEntry(LoggingEventType.Trace, "Agent Initilaisiert");
        Logger.Log(entry);
    }

    #endregion

    #region Tick

    public void Tick()
    {
        // Lese Zustandsparameter aus
        var parameters = _core.GetStateParameter();

        // Bestimme den Zustand der Tello Drohne
        if (parameters != null)
        {
            DroneState state = _stateDeterminer.DetermineState(parameters);
            // Log state
            //Console.WriteLine($"Current Drone state is: {state.ToString()}");
            
            // Zustand in die Simulation überführen
            //MapParameters(state, parameters);
        }
        
        // Entscheidungsfindungs
        _prevParameters = parameters;
        _tickCount++;
    }

    #endregion
    
    #region Private Methods

    private void MapParameters(DroneState currentState, TelloStateParameter parameters)
    {
        if (_prevParameters == null || currentState == DroneState.Unknown)
        {
            return;
        }
        
        double timeDifferenceSinceLastUpdate = parameters.UpdateTime.Millisecond - _prevParameters.UpdateTime.Millisecond;

        if ((currentState == DroneState.MovingForwards ||
             currentState == DroneState.MovingBackwards ||
             currentState == DroneState.MovingLeft ||
             currentState == DroneState.MovingRight) &&
            currentState == _prevDroneState)
        {
            _travelDuration += timeDifferenceSinceLastUpdate;
        }
        
        // Calculate bearing
        _bearing = DataMapper.CalculateBearing(currentState, _bearing, timeDifferenceSinceLastUpdate, _speed);
        double motionBearing = DataMapper.GetMotionBearing(currentState);
        double motionDirection = (_bearing + motionBearing) % 360;

        // calculate travelled distance
        double travelledDistance = 0;
        float acceleationDirection = 0;
        
        if (_currentDroneState == DroneState.MovingForwards || _currentDroneState == DroneState.MovingBackwards)
        {
            acceleationDirection = parameters.AccelerationX;
        }
        else if (_currentDroneState == DroneState.MovingRight || _currentDroneState == DroneState.MovingLeft)
        {
            acceleationDirection = parameters.AccelerationY;
        }
        
        travelledDistance = DataMapper.CalculateTravelledDistance(currentState, acceleationDirection, _travelDuration, timeDifferenceSinceLastUpdate);
        
        // move the agent to the corresponding position of the drone.
        Position = _layer._landScapeEnvironment.Move(this, motionDirection, travelledDistance); // nicht geprüft
        Console.WriteLine($"Agent moved to {Position}");

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