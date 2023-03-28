using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using RyzeTelloSDK.Enum;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, IPositionable
{
    #region Properties and Fields
  
    private TelloState _telloState;
    private TelloCore _core;
    private DateTime _lastStateUpdateTS;

    private LandScapeLayer _layer;
    private List<Position> _directions;
    private double _bearing;

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

    #region Inittialization
    
    public void Init(LandScapeLayer layer)
    {
        _layer = layer;

        _core = new TelloCore();
        _lastStateUpdateTS = DateTime.Now;
        Position = new Position(StartX, StartY);
    }

    #endregion

    #region Tick

    public void Tick()
    {
        _telloState = _core.GetState();
        
        // Drohnenzustand auf Simulation übertragen
        
        // Situation ermitteln und Einordnen
        
        // Action ausführen.
        
        if (_tickCount == 0)
        {
            var command = new DroneCommand(TelloAction.TakeOff, 0);
            _core.QueryCommand(command);
        }
        else if (_telloState.TOF >= 1)
        {
            var command = new DroneCommand(TelloAction.Land, 0);
            _core.QueryCommand(command);
        }

        //var state = _core.GetState();
        // Console.WriteLine("Running");
        _tickCount++;
    }

    #endregion
    
    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
}