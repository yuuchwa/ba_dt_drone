using DigitalTwinOfATelloDrone.Model.Attributes;
using DigitalTwinOfATelloDrone.Model.Services;
using DigitalTwinOfATelloDrone.RyzeSDK.Models;
using NUnit.Framework;

namespace DigitalTwinOfATelloDrone.Tests;

public class StateDeterminerTests
{
    [Test]
    public void TestInStandbyState()
    {
        // Arrange
        var determiner = StateDeterminer.getStateDeterminerInstance();
        var parameters = new TelloStateParameter();

        // Act
        DroneState state = determiner.DetermineState(parameters);

        // Assert
    }
}