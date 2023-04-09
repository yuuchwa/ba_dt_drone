using System.Linq;
using DtTelloDrone.Model.Agent;
using DtTelloDrone.Model.Entities;
using Mars.Components.Environments.Cartesian;
using Mars.Components.Layers;
using Mars.Core.Data;
using Mars.Interfaces.Data;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;

namespace DtTelloDrone.Model.Layer;

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

        foreach (var feature in Features)
        {
            var type = feature.VectorStructured.Data["polygon_type"].ToString();

            if (type is null)
            {
                continue;
            }

            if (type.Equals("boundingbox"))
            {
                var boundingBoxGeometry = feature.VectorStructured.Geometry;
                double xLowerLeft = boundingBoxGeometry.Coordinates[0].X;
                double yLowerLeft = boundingBoxGeometry.Coordinates[0].Y;
                double xUpperRight = boundingBoxGeometry.Coordinates[1].X;
                double yUpperRight = boundingBoxGeometry.Coordinates[1].Y;
                
                Position lowerLeftCoordinate = new(xLowerLeft, yLowerLeft);
                Position upperRightCoordinate = new(xUpperRight, yUpperRight);

                _landScapeEnvironment.BoundingBox = new BoundingBox(
                    lowerLeftCoordinate, 
                    upperRightCoordinate 
                );
            }
            else if (type.Equals("wall"))
            {
                var wall = new Wall(this);
                wall.Geometry = feature.VectorStructured.Geometry;
                _landScapeEnvironment.Insert(wall, feature.VectorStructured.Geometry);
            }
        }
    }
}