using System.Collections.Generic;
using System.Linq;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;

namespace GeoVectorBlueprint.Model;

/// <summary>
///     The GraphLayer holds a street network on which agents can move.
/// </summary>
public class GraphLayer : AbstractLayer
{
    #region Properties and Fields

    /// <summary>
    ///     The environment in which the agents can navigate.
    /// </summary>
    public SpatialGraphEnvironment Environment { get; private set; }

    /// <summary>
    ///     A list of agent instances that live on the layer.
    /// </summary>
    public List<Human> Humans { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    ///     The initialization method of the GraphLayer generates the Environment from the given input data and spawns
    ///     Human agents
    /// </summary>
    /// <param name="layerInitData">The initialization data for the layer provided by the configuration</param>
    /// <param name="registerAgentHandle">A handle for registering agents on the layer</param>
    /// <param name="unregisterAgent">A handle for unregistering agents on the layer</param>
    /// <returns>boolean that states that agent registration was successful</returns>
    public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle = null,
        UnregisterAgent unregisterAgent = null)
    {
        base.InitLayer(layerInitData, registerAgentHandle, unregisterAgent);

        var file = layerInitData.LayerInitConfig.File;
        // Environment = new SpatialGraphEnvironment(file); TODO sufficient for graphml
        Environment = new SpatialGraphEnvironment(new SpatialGraphOptions
        {
            GraphImports = new List<Input>
            {
                new()
                {
                    File = file,
                    InputConfiguration = new InputConfiguration
                    {
                        IsBiDirectedImport = true
                    }
                }
            }
        });

        // TODO export the current environment to a geojson file
        // File.WriteAllText("exported.geojson",Environment.ToGeoJson());

        var agentManager = layerInitData.Container.Resolve<IAgentManager>();
        Humans = agentManager.Spawn<Human, GraphLayer>().ToList();

        return true;
    }

    #endregion
}