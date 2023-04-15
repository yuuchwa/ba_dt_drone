using System;
using System.Numerics;
using static DtTelloDrone.Model.Services.TelloFlightMetrics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Model.Services;

/// <summary>
/// This class support the agent with auxillary methods.
/// </summary>
public static class DataMapper
{
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
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The angle</returns>
    public static double CalculateAngleOfTwoVectors(double[,] vec1, double[,] vec2)
    {
        double[,] sumVec = CalculateSumVector(vec1, vec2);
        double dotProduct = CalculateDotProduct(vec1, sumVec);
        double magnitude = CalculateMagnitude(vec1, sumVec);

        double result = 0;

        if (magnitude == 0)
        {
            return result;
        }

        double phi = Math.Round(Math.Acos(dotProduct / magnitude) * 180 / Math.PI, 3);
        
        if (0 < sumVec[0, 0] && 0 < sumVec[1, 0])
        {
            result = phi;
        }
        else if(sumVec[0, 0] < 0 && 0 < sumVec[1, 0])
        {
            result = 180 - phi;
        }
        else if (sumVec[0, 0] < 0 && sumVec[1, 0] < 0)
        {
            result = 180 + phi;
        }
        else if (0 < sumVec[0, 0] && sumVec[1, 0] < 0)
        {
            result = 360 - phi;
        }
        
        return result;
    }

    /// <summary>
    /// Calculates the dot product of two given vectors.
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The result the dot product.</returns>
    public static double CalculateDotProduct(double[,] vec1, double[,] vec2)
    {
        return vec1[0, 0] * vec2[0, 0] + vec1[1, 0] * vec2[1, 0];
    }

    public static double CalculateMagnitude(double[,] vec1, double[,] vec2)
    {
        return Math.Sqrt(vec1[0, 0] * vec1[0, 0] + vec1[1, 0] * vec1[1, 0]) *
               Math.Sqrt(vec2[0, 0] * vec2[0, 0] + vec2[1, 0] * vec2[1, 0]);
    }


    /// <summary>
    /// Calculates the angle between two given vectors.
    /// </summary>
    /// <param name="vec1">Vector 1</param>
    /// <param name="vec2">Vector 2</param>
    /// <returns>The sum vector.</returns>
    private static double[,] CalculateSumVector(double[,] vec1, double[,] vec2)
    {
        double[,] sumVec = {{vec1[0, 0] + vec2[0, 0]}, {vec1[1, 0] + vec2[1, 0]}};
        return sumVec;
    }
}