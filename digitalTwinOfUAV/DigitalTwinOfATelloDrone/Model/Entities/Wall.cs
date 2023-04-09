using DigitalTwinOfATelloDrone.Model.Layer;
using Mars.Components.Environments.Cartesian;

namespace DigitalTwinOfATelloDrone.Model.Entities;

public class Wall : Block
{
    public Wall(LandScapeLayer layer) : base(layer)
    {
    }

    public override CollisionKind? HandleCollision(ICharacter character)
    {
        return CollisionKind.Block;
    }
}