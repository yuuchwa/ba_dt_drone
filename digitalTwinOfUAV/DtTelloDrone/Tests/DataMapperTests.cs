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

    [Test]
    public void TestCalculateAngleOfTwoVektors()
    {
        double speedX = 6;
        double speedY = 3;
        
        double[,] vec1 = {{speedX},{0}};
        double[,] vec2 = {{0},{speedY}};

        double expectedResult = 26.565;
        double result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);
        
        speedX = -6;
        speedY = 3;

        vec1[0, 0] = speedX;
        vec2[1, 0] = speedY;

        expectedResult = 180 - 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);
        
        speedX = -6;
        speedY = -3;

        vec1[0, 0] = speedX;
        vec2[1, 0] = speedY;

        expectedResult = 180 + 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);
        
        speedX = 6;
        speedY = -3;

        vec1[0, 0] = speedX;
        vec2[1, 0] = speedY;

        expectedResult = 360 - 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void TestCalculateSpeed()
    {
        double timeInterval = 0.002;
        double acceleration = 40;
        double initialVelocity = 5;

        var expectedResult = 0.4;
        var result = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void TestCalculateDotProduct()
    {
        double speedX = 6;
        double speedY = 3;
        
        double[,] vec1 = {{speedX},{0}};
        double[,] vec2 = {{0},{speedY}};
    }

    [Test]
    public void TestCalculateMotionBearingValidInput()
    {
        /* Test 1 */
        
        double speedX = 6;
        double speedY = 3;
        
        double[,] vec1 = {{speedX},{0}};
        double[,] vec2 = {{0},{speedY}};
        
        double expectedResult = 26.565;
        var result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);

        /* Test 2 */
        
        speedX = -6;
        speedY = 3;

        vec1[0, 0] = speedX;
        vec1[1, 0] = speedY;

        expectedResult = 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result);
    }


}