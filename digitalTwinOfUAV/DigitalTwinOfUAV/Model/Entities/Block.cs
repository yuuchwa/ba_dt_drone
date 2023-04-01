using System;
using DigitalTwinOfUAV.Model.Layer;
using Mars.Components.Environments.Cartesian;

namespace DigitalTwinOfUAV.Model.Entities;

public abstract class Block : IObstacle
{
    protected LandScapeLayer Battleground { get; }
    
    public Block(LandScapeLayer layer)
    {
        Battleground = layer;
    }

    public Guid ID { get; set; }
    public bool IsRoutable(ICharacter character)
    {
        return false;
    }

    public CollisionKind? HandleCollision(ICharacter character)
    {
        return CollisionKind.Block;
    }

    public VisibilityKind? HandleExploration(ICharacter explorer)
    {
        return VisibilityKind.Opaque;
    }
}