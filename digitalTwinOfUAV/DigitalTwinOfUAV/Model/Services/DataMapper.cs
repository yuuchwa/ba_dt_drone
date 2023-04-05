using System;
using System.Text.RegularExpressions;
using DigitalTwinOfUAV.Model.Attributes;

namespace DigitalTwinOfUAV.Model.Services;

/// <summary>
/// This auxiliary class helps the agent to c
/// </summary>
public static class DataMapper
{
    /// <summary>
    /// Calculates the distance in mm in which the drone is travelled.
    /// </summary>
    /// <returns></returns>
    public static double CalculateTravelledDistance()
    {
        
        return 0;
    }

    /// <summary>
    /// calculates the direction the drone is oriented.
    /// </summary>
    /// <returns></returns>
    public static double CalculateBearing(double oldBearing, DroneState state, double timeInterval, int angleVelocity, float droneLenth)
    {
        if (state == DroneState.RotatingClockwise || state == DroneState.RotatingCounterClockwise)
        {
            return oldBearing;
        }
        
        double newBearing = oldBearing;
        double RotationAngle = Math.Atan(angleVelocity * timeInterval / droneLenth) % 360;
        
        if (state == DroneState.RotatingCounterClockwise)
        {
            newBearing += RotationAngle;
        }
        else if (state == DroneState.RotatingClockwise)
        {
            newBearing -= RotationAngle;
        }
        
        return newBearing;
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