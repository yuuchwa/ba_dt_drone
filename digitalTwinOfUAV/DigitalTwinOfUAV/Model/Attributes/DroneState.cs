using System;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Attributes;

/// <summary>
/// Enum which indicates the current action.
/// </summary>
public enum DroneState
{
    Unknown,
    Standby,
    Hovering,
    TakingOff,
    Landing,
    MovingForwards,
    MovingBackwards,
    MovingLeft,
    MovingRight,
    RotatingClockwise,
    RotatingCounterClockwise,
    Rising,
    Sinking,
}