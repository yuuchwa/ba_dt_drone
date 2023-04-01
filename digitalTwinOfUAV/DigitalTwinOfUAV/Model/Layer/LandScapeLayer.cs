using System.Collections.Generic;
using System.Linq;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Entities;
using Mars.Components.Environments;
using Mars.Components.Environments.Cartesian;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Core.Data;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace DigitalTwinOfUAV.Model.Layer;

public class LandScapeLayer : VectorLayer
{
    private readonly Position[] areaSize =
    {
        // Ungefähre Größe meines Raumes. In Zukunft sollten diese Maße über eine geojson übergeben werden.
        new(0, 0),
        new(40, 30)
    };
    
    public IAgentManager AgentManager { get; private set; }

    public CollisionEnvironment<TelloAgent, Block> _landScapeEnvironment { get; private set; }

    public override bool InitLayer(
        LayerInitData layerInitData,
        RegisterAgent registerAgentHandle,
        UnregisterAgent unregisterAgentHandle)
    {
        var initiated = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);

        BuildEnvironment();
        
        AgentManager = layerInitData.Container.Resolve<IAgentManager>();
        
        var telloAgent = AgentManager.Spawn<TelloAgent, LandScapeLayer>().ToList();
        
        return initiated;
    }

    private void BuildEnvironment()
    {
        _landScapeEnvironment = new CollisionEnvironment<TelloAgent, Block>();

        _landScapeEnvironment.BoundingBox = new BoundingBox(
            areaSize[0],
            areaSize[1]
        );
    }
}