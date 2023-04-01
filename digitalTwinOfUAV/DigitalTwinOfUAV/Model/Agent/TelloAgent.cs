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

    private TelloCore _core;
    private StateDeterminer _stateDeterminer;

    private DateTime _lastStateUpdateTimeStamp;
    private TelloStateParameter _currentParameter;
    private TelloStateParameter _prevParameters;
    
    private DroneState _currentDroneState;
    private DroneState _prevDroneState;
    
    private List<Position> _directions;
    private double _bearing;
    private byte _speed;

    private int _tickCount = 0;
    private Random _random = new();
    
    
    /// <summary>
    /// The unique identifier of the agent.
    /// </summary>
    public Guid ID { get; set; }

    [PropertyDescription (Name ="StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription (Name = "StartY")]
    public int StartY { get; set; }
    
    /// <summary>
    /// The Distance a UAV should move.
    /// </summary>
    public double Distance { get; set; }

    public Position Position { get; set; }
    
    public Position Target { get; set; }
    
    #endregion

    #region Constants
    
    private const int DefaultSpeed = 30; 

    #endregion

    #region Initialization
    
    public void Init(LandScapeLayer layer)
    {
        _layer = layer;

        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
            
        _core = new TelloCore();
        _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
        _speed = DefaultSpeed;

        _lastStateUpdateTimeStamp = DateTime.Now;
    }

    #endregion

    #region Tick

    public void Tick()
    {
        _currentParameter = _core.GetStateParameter();
        _stateDeterminer.DetermineState(_currentParameter);
        _lastStateUpdateTimeStamp = DateTime.Now;

        readKeyboard();
        
        _tickCount++;
    }

    #endregion
    
    #region Private Methods

    private void readKeyboard()
    {
        DroneCommand command = null;
        var key = Console.ReadKey(true);

        if (key != null)
        {
            Console.WriteLine($"{key.Key} pressed");

            switch (key.Key)
            {
                case ConsoleKey.W: command = new DroneCommand(TelloAction.MoveForward, _speed); break;
                case ConsoleKey.S: command = new DroneCommand(TelloAction.MoveBackward, _speed); break;
                case ConsoleKey.A: command = new DroneCommand(TelloAction.MoveLeft, _speed); break;
                case ConsoleKey.D: command = new DroneCommand(TelloAction.MoveRight, _speed); break;
                case ConsoleKey.R: command = new DroneCommand(TelloAction.Rise, _speed); break;
                case ConsoleKey.F: command = new DroneCommand(TelloAction.Sink, _speed); break;
                case ConsoleKey.Q: command = new DroneCommand(TelloAction.RotateLeft, _speed); break;
                case ConsoleKey.E: command = new DroneCommand(TelloAction.RotateRight, _speed); break;
                case ConsoleKey.Spacebar: command = new DroneCommand(TelloAction.Stop, 0); break;
            
                case ConsoleKey.T: command = new DroneCommand(TelloAction.TakeOff, 0); break;
                case ConsoleKey.L: command = new DroneCommand(TelloAction.Land, 0); break;
                case ConsoleKey.P: command = new DroneCommand(TelloAction.Emergency, 0); break;
            
                case ConsoleKey.B: command = new DroneCommand(TelloAction.Battery, 0); break;
                default: break;
            }

            if (command._action != null)
            {
                _core.QueryCommand(command);
            }
        }
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