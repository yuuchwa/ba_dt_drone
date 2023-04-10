using DtTelloDrone.Model.Services;
using NUnit.Framework;

namespace DtTelloDrone.Tests;

public class DataMapperTests
{
    [Test]
    public void TestMapToMarsBearingValidInput()
    {
        double inputYaw = 0;
        double expectedBearing = 0;
        var result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 0.2;
        expectedBearing = 0;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 0.6;
        expectedBearing = 359;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -0.2;
        expectedBearing = 0;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -0.6;
        expectedBearing = 1;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 45;
        expectedBearing = 315;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 45;
        expectedBearing = 315;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 90;
        expectedBearing = 270;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 179;
        expectedBearing = 181;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -179;
        expectedBearing = 179;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);

        inputYaw = 179.2;
        expectedBearing = 181;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -179.2;
        expectedBearing = 179;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -90;
        expectedBearing = 90;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -45;
        expectedBearing = 45;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
    }

    [Test]
    public void TestMapToMarsBearingInvalidInput()
    {
        double inputYaw = 180;
        double expectedBearing = -1;
        var result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 179.5;
        expectedBearing = -1;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -180;
        expectedBearing = -1;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = 181;
        expectedBearing = -1;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
        
        inputYaw = -180.2;
        expectedBearing = -1;
        result = DataMapper.MapToMarsBearing(inputYaw);
        Assert.AreEqual(expectedBearing, result);
    }
}