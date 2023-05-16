using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.TelloSdk.DataModels;
using NUnit.Framework;

namespace DtTelloDrone.Tests;

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