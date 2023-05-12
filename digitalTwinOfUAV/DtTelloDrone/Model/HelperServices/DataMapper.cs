using System;
using MathNet.Numerics.LinearAlgebra;
using static DtTelloDrone.TelloSdk.Attribute.TelloFlightMetrics;

namespace DtTelloDrone.Model.HelperServices;

/// <summary>
/// This static class provides auxiliary methods to support the agent with
/// various mathematical calculations related to drone movement and direction.
/// </summary>
public static class DataMapper
{
    private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Maps a yaw value to a corresponding Mars bearing value.
    /// </summary>
    /// <param name="yaw">A value representing the yaw value.</param>
    /// <returns>A value representing the corresonding MARS bearing angle.</returns>
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
    /// This method maps a given bearing to its corresponding coordinate in Mars.
    /// </summary>
    /// <param name="bearing">he bearing in degrees.</param>
    /// <returns>The corresponding coordinate in Mars.</returns>
    public static double MapCoordinateToMarsCoordinate(double bearing)
    {
        return (360 - bearing) % 360;
    }

    /// <summary>
    /// /// Calculates the speed of the drone based on the given time interval, acceleration and initial velocity.
    /// </summary>
    /// <param name="timeInterval">The time interval in seconds</param>
    /// <param name="acceleration">Acceleration in mm/s^2</param>
    /// <param name="initialVelocity">The initial velocity in mm/s</param>
    /// <returns></returns>
    public static double CalculateSpeed(double timeInterval, double acceleration, double initialVelocity)
    {
        double speed = acceleration * timeInterval + initialVelocity;
        return Math.Round(speed, 3);
    }

    /// <summary>
    /// Calculates the angle between two given vectors using the dot product formula.
    /// https://www.cuemath.com/geometry/angle-between-vectors/
    /// </summary>
    /// <param name="vec1">The first vector.</param>
    /// <param name="vec2">The second vector.</param>
    /// <returns>The angle in degrees</returns>
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
    /// This method calculates the magnitude of a given vector.
    /// </summary>
    /// <param name="vec">the vector.</param>
    /// <returns>The magnitude</returns>
    public static double CalculateMagnitude(Vector<double> vec)
    {
        return Math.Sqrt(vec.At(0) * vec.At(0) + vec.At(1) * vec.At(1));
    }

    /// <summary>
    /// Calculates the direction of movement relative to the environment.
    /// </summary>
    /// <param name="bearingSelf">The actual bearing of the drone.</param>
    /// <param name="bearingMotion">The direction of the movement relative to the bearing of the drone</param>
    /// <returns>The direction in degrees.</returns>
    public static double CalculateFlyDirection(double bearingSelf, double bearingMotion)
    {
        return (bearingMotion + bearingSelf) % 360;
    }
}