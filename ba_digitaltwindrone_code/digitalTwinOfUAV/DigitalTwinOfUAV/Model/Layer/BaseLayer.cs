using System.Collections.Generic;
using System.Linq;
using DigitalTwinOfUAV.Model.Agent;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace DigitalTwinOfUAV.Model.Layer;

public class BaseLayer : VectorLayer
{
    /// <summary>
    ///     A list of agent instances that live on the layer.
    /// </summary>
    public List<TelloAgent> UAVs { get; set; }
    public IAgentManager AgentManager { get; private set; }
    
    
    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        var hiders = AgentManager.Spawn<TelloAgent, BaseLayer>().ToList();
        //var seekers = AgentManager.Spawn<GastSeeker, ArenaLayer>().ToList();
        
        return initiated;
    }

}