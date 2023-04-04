using System;
using System.Collections.Generic;
using DigitalTwinOfUAV.Model.Attributes;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.Model.Services;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using RyzeTelloSDK.Enum;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter
{
    #region Properties and Fields

    private LandScapeLayer _layer;

    private ICore _core;
    private StateDeterminer _stateDeterminer;

    private TelloStateParameter _prevParameters;
    
    private DroneState _currentDroneState;
    private DroneState _prevDroneState;

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
        _layer = layer;
        _core = TelloCore.GetTelloCoreInstance();
        
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
            
        _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
        _speed = DefaultSpeed;
        _bearing = DefaultBearing;
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
            //DroneState state = _stateDeterminer.DetermineState(parameters);
            //Console.WriteLine($"Current Drone state is: {state.ToString()}");
            
            // Zustand in die Simulation überführen
            //MapParameters(state, parameters);
        }
        
        // Neuen Befehl einlesen
        TelloAction selectedAction = readKeyboard();
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
        }
        
        _prevParameters = parameters;
        _tickCount++;
    }

    #endregion
    
    #region Private Methods

    private void MapParameters(DroneState droneState, TelloStateParameter parameters)
    {
        if (_prevParameters == null)
        {
            return;
        }
        
        if (droneState == DroneState.RotatingClockwise || droneState == DroneState.RotatingClockwise)
        {
            _bearing = CalculateBearing();
        }
        else if (droneState == DroneState.MovingBackwards)
        {
            double timeDifferenceSinceLastUpdate = parameters.UpdateTime.Second - _prevParameters.UpdateTime.Second;
            double travelledDistance = _speed * timeDifferenceSinceLastUpdate / 1000;
            Position = _layer._landScapeEnvironment.Move(this, _bearing, travelledDistance); // nicht geprüft
            Console.WriteLine($"Agent moved to {Position}");

        }

        _height = _height;
    }

    private double CalculateBearing()
    {
        return 0;
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

    private TelloAction readKeyboard()
    {
        TelloAction action = TelloAction.Unknown;
        var key = Console.ReadKey(true);

        if (key != null)
        {
            Console.WriteLine($"{key.Key} pressed");

            switch (key.Key)
            {
                case ConsoleKey.W: action = TelloAction.MoveForward; break;
                case ConsoleKey.S: action = TelloAction.MoveBackward; break;
                case ConsoleKey.A: action = TelloAction.MoveLeft; break;
                case ConsoleKey.D: action = TelloAction.MoveRight; break;
                case ConsoleKey.R: action = TelloAction.Rise; break;
                case ConsoleKey.F: action = TelloAction.Sink; break;
                case ConsoleKey.Q: action = TelloAction.RotateLeft; break;
                case ConsoleKey.E: action = TelloAction.RotateRight; break;
                case ConsoleKey.Spacebar: action = TelloAction.Stop; break;
            
                case ConsoleKey.T: action = TelloAction.TakeOff; break;
                case ConsoleKey.L: action = TelloAction.Land; break;
                case ConsoleKey.P: action = TelloAction.Emergency; break;
            
                case ConsoleKey.B: action = TelloAction.Battery; break;
                case ConsoleKey.C: action = TelloAction.Connect; break;
                default: break;
            }
        }
        return action;

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