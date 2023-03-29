using System.Collections.Generic;
using System.Linq;
using DigitalTwinOfUAV.Model.Agent;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;

namespace DigitalTwinOfUAV.Model.Layer;

public class LandScapeLayer : RasterLayer
{
    public IAgentManager AgentManager { get; private set; }

    public SpatialHashEnvironment<TelloAgent> _landScapeEnvironment;

    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        _landScapeEnvironment = new SpatialHashEnvironment<TelloAgent>(Width, Height);
        
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        
        var telloAgent = AgentManager.Spawn<TelloAgent, LandScapeLayer>().ToList();
        
        return initiated;
    }

}