using System;
using DigitalTwinOfATelloDrone.Model.Layer;
using Mars.Components.Environments.Cartesian;
using NetTopologySuite.Geometries;

namespace DigitalTwinOfATelloDrone.Model.Entities;

public abstract class Block : IObstacle
{
    protected LandScapeLayer Battleground { get; }
    
    public Geometry Geometry { get; set; }
    
    public Block(LandScapeLayer layer)
    {
        Battleground = layer;
    }

    public Guid ID { get; set; }
    public bool IsRoutable(ICharacter character)
    {
        return false;
    }

    public virtual CollisionKind? HandleCollision(ICharacter character)
    {
        return CollisionKind.Block;
    }

    public virtual VisibilityKind? HandleExploration(ICharacter explorer)
    {
        return VisibilityKind.Opaque;
    }
}