using System;
using Mars.Common.Core;
using static DtTelloDrone.Model.Services.TelloFlightMetrics;
using MathNet.Numerics.LinearAlgebra;
using NLog;

namespace DtTelloDrone.Model.Services;

/// <summary>
/// This class support the agent with auxillary methods.
/// </summary>
public static class DataMapper
{
    private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static double MapToMarsBearing(double yaw)
    {
        double marsBearing = -1;
        int intYaw = Convert.ToInt32(yaw);

        if (intYaw == InitialYaw)
        {
            marsBearing = 0;
        }
        else if (MinYawDegree <= intYaw && intYaw < InitialYaw)
        {
            marsBearing = -1 * intYaw;
        }
        else if (InitialYaw < intYaw && intYaw <= MaxYawDegree)
        {
            marsBearing = 360 - intYaw;
        }

        return Math.Truncate(marsBearing);
    }

    public static double MapNormalCoordinateToMars(double bearing)
    {
        return (360 - bearing) % 360;
    }

    /// <summary>
    /// Calculates the distance in mm in which the drone is travelled.
    /// </summary>
    /// <param name="timeInterval">Time in seconds</param>
    /// <param name="acceleration">Acceleration in mm/s^2</param>
    /// <param name="initialVelocity"></param>
    /// <returns></returns>
    public static double CalculateSpeed(double timeInterval, double acceleration, double initialVelocity)
    {
        double speed = acceleration * timeInterval + initialVelocity;
        return Math.Round(speed, 3);
    }

    /// <summary>
    /// Calculates the angle of two given vectors.
    /// https://www.cuemath.com/geometry/angle-between-vectors/
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The angle</returns>
    public static double CalculateAngleOfTwoVectors(Vector<double> vec1, Vector<double> vec2)
    {
        if (CalculateMagnitude(vec1) == 0 && CalculateMagnitude(vec2) == 0)
        {
            return 0;
        }
        
        if (1 <= CalculateMagnitude(vec1)&& CalculateMagnitude(vec2) == 0)
        {
            if (0 < vec1.At(0))
            {
                return 0;
            }
            else if (vec1.At(0) < 0)
            {
                return 180;
            }
        }

        if (CalculateMagnitude(vec1) == 0 && 1 <= CalculateMagnitude(vec2))
        {
            if (0 < vec2.At(1))
            {
                return 90;
            }
            else if (vec2.At(1) < 0)
            {
                return 270;
            }
        }

        Vector<double> sumVec = vec1 + vec2;
        double dotProduct = vec1.DotProduct(sumVec);
        double magnitude = CalculateMagnitude(vec1) * CalculateMagnitude(sumVec);
        
        double result = 0;

        if (magnitude == 0)
        {
            return result;
        }

        double phiRad = Math.Round( Math.Acos(dotProduct / magnitude), 3);

        double phiDegree = phiRad * (180 / Math.PI);
        
        if (0 <= sumVec.At(0) && 0 <= sumVec.At(1))
        {
            result = phiDegree;
        }
        else if(sumVec.At(0) <= 0 && 0 <= sumVec.At(1))
        {
            result = 180 - phiDegree;
        }
        else if (sumVec.At(0) <= 0 && sumVec.At(1) <= 0)
        {
            result = 180 + phiDegree;
        }
        else if (0 <= sumVec.At(0) && sumVec.At(1) <= 0)
        {
            result = 360 - phiDegree;
        }
        
        return result;
    }

    /// <summary>
    /// Calculates the dot product of two given vectors.
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The result the dot product.</returns>
    public static double CalculateDotProduct(Vector<double> vec1, Vector<double> vec2)
    {
        return vec1.At(0) * vec2.At(0) + vec1.At(1) * vec2.At(1);
    }

    public static double CalculateMagnitude(Vector<double> vec)
    {
        return Math.Sqrt(vec.At(0) * vec.At(0) + vec.At(1) * vec.At(1));
    }

    /// <summary>
    /// Calculates the angle between two given vectors.
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The sum vector.</returns>
    private static Vector<double> CalculateSumVector(Vector<double> vec1, Vector<double> vec2)
    {
        return null;
    }

    public static double CalculateFlyDirection(double bearingSelf, double bearingMotion)
    {
        return (bearingMotion + bearingSelf) % 360;
    }
}