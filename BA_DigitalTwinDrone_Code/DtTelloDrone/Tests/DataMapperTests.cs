using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.HelperServices;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Tests;

public class DataMapperTests
{
    [Test]
    public void MapToMarsBearing_ValidInput()
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
    public void MapToMarsBearing_InvalidInput()
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
    public void CalculateAngleOfTwoVectors_VecXisGreateVecY()
    {
        double speedX = 6;
        double speedY = 3;;
        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0, speedY});;
        double expectedResult = 26.565;;
        double result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);;

        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -6;
        speedY = 3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 153.435; // 180 - 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -6;
        speedY = -3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 206.565; // 180 + 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = 6;
        speedY = -3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 333.435; //360 - 26.565;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
    }

    [Test]
    public void CalculateAngleOfTwoVectors_VecYisGreateVecX()
    {
        double speedX = 3;
        double speedY = 6;;
        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0, speedY});;
        double expectedResult = 63.435;
        double result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);;

        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -3;
        speedY = 6;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 180 - 63.435;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -3;
        speedY = -6;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 180 + 63.435;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = 3;
        speedY = -6;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 360 - 63.435;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
    }
    
    [Test]
    public void CalculateAngleOfTwoVectors_VecYSameLengthAsVecX()
    {
        double speedX = 3;
        double speedY = 3;;
        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0, speedY});;
        double expectedResult = 45;
        double result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);;

        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -3;
        speedY = 3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 180 - 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = -3;
        speedY = -3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 180 + 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
        
        speedX = 3;
        speedY = -3;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 360 - 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
    }

    [Test]
    public void CalculateAngleOfTwoVectors_VecYAndVecXHasSameRatio()
    {
        double speedX = 3;
        double speedY = 3;
        ;
        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0, speedY});
        ;
        double expectedResult = 45;
        double result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        ;

        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 6;
        speedY = 6;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 9;
        speedY = 9;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 5;
        speedY = 5;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 45;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 7;
        speedY = 2;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 15.945;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 14;
        speedY = 4;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 15.945;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 21;
        speedY = 6;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 15.945;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 4;
        speedY = 14;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 74.055;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);

        speedX = 2;
        speedY = 7;
        vec1.Storage[0] = speedX;
        vec2.Storage[1] = speedY;
        expectedResult = 74.055;
        result = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        Assert.AreEqual(expectedResult, result, 0.1);
    }

    [Test]
    public void CalculateFlyDirection_PositivInputs()
    {
        double bearingOne = 0;
        double bearingTwo = 0;
        double expectedResult = 0;
        double result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 1;
        bearingTwo = 1;
        expectedResult = 2;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 90;
        bearingTwo = 90;
        expectedResult = 180;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 180;
        bearingTwo = 180;
        expectedResult = 0;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 270;
        bearingTwo = 90;
        expectedResult = 0;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 359;
        bearingTwo = 90;
        expectedResult = 89;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 1;
        bearingTwo = 90;
        expectedResult = 91;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 315;
        bearingTwo = 90;
        expectedResult = 45;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void CalculateFlyDirection_NegativeInputs()
    {        
        double bearingOne = 45;
        double bearingTwo = 180;
        double expectedResult = 225;
        double result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 225;
        bearingTwo = 180;
        expectedResult = 45;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 199;
        bearingTwo = 48;
        expectedResult = 247;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 87;
        bearingTwo = 247;
        expectedResult = 334;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
        
        bearingOne = 270;
        bearingTwo = 270;
        expectedResult = 180;
        result = DataMapper.CalculateFlyDirection(bearingOne, bearingTwo);
        Assert.AreEqual(expectedResult, result);
    }
    
    [Test]
    public void CalculateSpeed_WithSmallInitialVelocity_ReturnsCorrectSpeed()
    {
        double timeInterval = 1.0;
        double acceleration = 2.0;
        double initialVelocity = 2.0;

        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);

        Assert.AreEqual(4.0, speed);
    }

    [Test]
    public void CalculateSpeed_WithLargeInitialVelocity_ReturnsInitialVelocity()
    {
        double timeInterval = 1.0;
        double acceleration = 2.0;
        double initialVelocity = 5.0;

        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);

        Assert.AreEqual(7.0, speed);
    }

    [Test]
    public void CalculateSpeed_WithNegativeInitialVelocity_ReturnsInitialVelocity()
    {
        double timeInterval = 1.0;
        double acceleration = 2.0;
        double initialVelocity = -4.0;

        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);

        Assert.AreEqual(-2.0, speed);
    }
    
    [Test]
    public void CalculateSpeed_WithZeroTimeInterval_ReturnsInitialVelocity()
    {
        double timeInterval = 0.0;
        double acceleration = 2.0;
        double initialVelocity = 3.0;
    
        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
    
        Assert.AreEqual(3.0, speed);
    }
    
    [Test]
    public void CalculateSpeed_WithNegativeAcceleration_ReturnsInitialVelocity()
    {
        double timeInterval = 1.0;
        double acceleration = -2.0;
        double initialVelocity = 4.0;
    
        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
    
        Assert.AreEqual(2.0, speed);
    }
    
    [Test]
    public void CalculateSpeed_WithZeroAcceleration_ReturnsInitialVelocity()
    {
        double timeInterval = 1.0;
        double acceleration = 0.0;
        double initialVelocity = 2.5;
    
        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
    
        Assert.AreEqual(2.5, speed);
    }
    
    [Test]
    public void CalculateSpeed_WithLargeAcceleration_ReturnsCorrectSpeed()
    {
        double timeInterval = 1.5;
        double acceleration = 3.0;
        double initialVelocity = 1.0;
    
        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
    
        Assert.AreEqual(5.5, speed);
    }
    
    [Test]
    public void CalculateSpeed_WithRoundedSpeed_ReturnsCorrectRoundedSpeed()
    {
        double timeInterval = 1.0;
        double acceleration = 2.0;
        double initialVelocity = -3.333;
    
        double speed = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
    
        Assert.AreEqual(-1.3330000000000002, speed);
    }
    
    [Test]
    public void MapActionToBearing_MoveForward_Returns0()
    {
        DroneAction action = DroneAction.MoveForward;

        double result = DataMapper.MapActionToBearing(action);

        Assert.AreEqual(0, result);
    }

    [Test]
    public void MapActionToBearingMoveLeftReturns270()
    {
        DroneAction action = DroneAction.MoveLeft;

        double result = DataMapper.MapActionToBearing(action);

        Assert.AreEqual(270, result);
    }

    [Test]
    public void MapActionToBearingMoveBackwardReturns180()
    {
        DroneAction action = DroneAction.MoveBackward;
        
        double result = DataMapper.MapActionToBearing(action);
        
        Assert.AreEqual(180, result);
    }

    [Test]
    public void MapActionToBearingMoveRightReturns80()
    {
        DroneAction action = DroneAction.MoveRight;
        
        double result = DataMapper.MapActionToBearing(action);
        
        Assert.AreEqual(80, result);
    }

    [Test]
    public void MapActionToBearingInvalidActionReturnsDefault()
    {
        DroneAction action = DroneAction.Land; // An invalid action
        double result = DataMapper.MapActionToBearing(action);
        
        Assert.AreEqual(0, result);

        action = DroneAction.TakeOff; // An invalid action
        result = DataMapper.MapActionToBearing(action);
        
        Assert.AreEqual(0, result);
    }

    /*
    [Test]
    public void TestCalculateSpeed()
    {
        double timeInterval = 0.002;
        double acceleration = 40;
        double initialVelocity = 5;

        var result = DataMapper.CalculateSpeed(timeInterval, acceleration, initialVelocity);
        Assert.AreEqual(expectedResult, result);
    }
    */
}