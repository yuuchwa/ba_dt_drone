using System;
using System.Text.RegularExpressions;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.RyzeSDK.Attribute;
using static DtTelloDrone.Model.Services.TelloFlightMetrics;

namespace DtTelloDrone.Model.Services;

/// <summary>
/// This auxiliary class helps the agent to c
/// </summary>
public static class DataMapper
{
    /// <summary>
    /// Calculates the distance in mm in which the drone is travelled.
    /// </summary>
    /// <returns></returns>
    public static double CalculateTravelledDistance(double timeInterval, double accelecation,  double velocity)
    {
        return (0.5 * accelecation * (timeInterval * timeInterval)) + (velocity * timeInterval); // 1/2 * m * s² + v0 * t
    }
    
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
    /// Calculates the bearing in which the drone is moving.
    /// </summary>
    /// <returns></returns>
    public static double GetMotionBearing(DroneState state)
    {
        double bearing = 0;
        switch (state)
        {
            case DroneState.Hovering: bearing = 0; break;
            case DroneState.MovingForwards: bearing = 0; break;    // x = 1, y = 0
            case DroneState.MovingLeft: bearing = 90; break;       // x = 0, y = 1
            case DroneState.MovingBackwards: bearing = 180; break; // x = -1, y = 0
            case DroneState.MovingRight: bearing = 270; break;     // x = -1, y = -1
            default: break;
        }
        return bearing;
    }
}