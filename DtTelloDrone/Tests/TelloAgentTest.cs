using System;
using DtTelloDrone.Model.HelperServices;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Tests;
using NUnit.Framework;

public class TelloAgentTest
{
    [SetUp]
    public void SetUp()
    {
        
    }

    /******************* Valid Inputs ****************************/
    
    [Test]
    public void UpdatePosition_DroneMovingForwardStraight()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = 0;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 4;
        double expectedSpeedY = 0;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 0;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 0;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 4; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingBackwardsStraight()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(40.0) * -1;       // cm/ms^2
        double accelerationY = 0;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = -4;
        double expectedSpeedY = 0;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 180;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 180;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 4; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingLeftStraight()
    {
        double timeInterval = 0.1;
        double accelerationX = 0;       // cm/ms^2
        double accelerationY = Math.Round(-40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 0;
        double expectedSpeedY = 4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});
        
        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 90;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double MotionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, MotionBearingMars);

        double expectedflyDirection = 270;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 4; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingRightStraight()
    {
        double timeInterval = 0.1;
        double accelerationX = 0;       // cm/ms^2
        double accelerationY = Math.Round(40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 0;
        double expectedSpeedY = -4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});
        
        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 270;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 90;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 4; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardDiagonalRight()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(-40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 4;
        double expectedSpeedY = 4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 45;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 315;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 8; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingBackwardsDiagonalRight()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = -4;
        double expectedSpeedY = -4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 225;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 135;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 8; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }

    [Test]
    public void UpdatePosition_DroneMovingForwardDiagonalLeft()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(-40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 4;
        double expectedSpeedY = 4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 45;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 315;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 8; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingBackwardDiagonalLeft()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(40.0) * -1;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = -4;
        double expectedSpeedY = -4;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 225;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 135;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 8; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardWithInitialBearing()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = 0;
        double velocityX = 0;             // cm/ms
        double velocityY = 0;
        double Bearing = 55;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 4;
        double expectedSpeedY = 0;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 0;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 55;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 4; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardWithInitialVelocityX()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = 0;
        double velocityX = 5;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 9;
        double expectedSpeedY = 0;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 0;
        Assert.AreEqual(expectedMotionBearing, motionBearing);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 0;
        Assert.AreEqual(expectedflyDirection, flyDirection);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 9; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardWithInitialVelocityY()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(-40.0) * -1;       // cm/ms^2
        double accelerationY = 0;
        double velocityX = 0;             // cm/ms
        double velocityY = 5;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = 4;
        double expectedSpeedY = 5;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 51.334;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 308.664;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 9; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardDiagonalRight_WithInitialVelocityYAndVelocityX()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(40.0) * -1;
        double velocityX = -6;             // cm/ms
        double velocityY = -8;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = -10;
        double expectedSpeedY = -12;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 230.191;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 129.808;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 22; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_DroneMovingForwardWithInitialVelocityYAndVelocityXAndBearing()
    {
        double timeInterval = 0.1;
        double accelerationX = Math.Round(40.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(40.0) * -1;
        double velocityX = -6;             // cm/ms
        double velocityY = -8;
        double Bearing = 66;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        double expectedSpeedX = -10;
        double expectedSpeedY = -12;

        Assert.AreEqual(expectedSpeedX, speedX);
        Assert.AreEqual(expectedSpeedY, speedY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);

        double expectedMotionBearing = 230.191;
        Assert.AreEqual(expectedMotionBearing, motionBearing, 0.1);

        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double expectedflyDirection = 195.808;
        Assert.AreEqual(expectedflyDirection, flyDirection, 0.1);

        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);

        double expectedTravelingDistance = 22; // cm
        Assert.AreEqual(expectedTravelingDistance, travelingDistance);
    }
    
    [Test]
    public void UpdatePosition_ReadData()
    {
        double timeInterval = 0.102;
        double accelerationX = Math.Round(28.0) * -1;       // cm/ms^2
        double accelerationY = Math.Round(-12.0) * -1;
        double velocityX = -2;             // cm/ms
        double velocityY = 0;
        double Bearing = 0;
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        Vector<double> vec1 = new DenseVector(new double[] {speedX, 0});
        Vector<double> vec2 = new DenseVector(new double[] {0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        
        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);
        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);
        
        double travelingDistance = DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2);
    }
}