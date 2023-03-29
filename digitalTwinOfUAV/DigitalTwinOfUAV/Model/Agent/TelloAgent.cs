using System;
using System.Collections.Generic;

using DigitalTwinOfUAV.Model.Attributes;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, IPositionable
{
    #region Properties and Fields

    private TelloCore _core;
    private StateDeterminer _stateDeterminer;

    private TelloStateParameter _currentParameter;
    private TelloStateParameter _prevParameters;
    
    private State _currentState;
    private State _prevState;
    
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

    #region Initialization
    
    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        Position = new Position(StartX, StartY);

        _core = new TelloCore();
        _stateDeterminer = StateDeterminer.getStateDeterminerinstance();
        
        _lastStateUpdateTS = DateTime.Now;
    }

    #endregion

    #region Tick

    public void Tick()
    {
        _currentParameter = _core.GetStateParameter();
        _stateDeterminer.DetermineState(_currentParameter);
        
        // Drohnenzustand auf Simulation übertragen
        
        // Situation ermitteln und Einordnen
        
        // Action ausführen.
        
        _tickCount++;
    }

    #endregion
    
    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
}