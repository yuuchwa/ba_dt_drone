using System;
using DigitalTwinOfUAV.Model.Layer;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Agent;

public class TelloAgent : IAgent<BaseLayer>
{
    private TelloState _telloState;
    private DateTime _lastStateUpdate;
    
    public Guid ID { get; set; }

    public void Init(BaseLayer layer)
    {
        _lastStateUpdate = DateTime.Now;
    }

    public void Tick()
    {
        
    }

    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
}