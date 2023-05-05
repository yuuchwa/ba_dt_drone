using System;

namespace DtTelloDrone.Model.Attributes;

/// <summary>
/// Enum which indicates the current action.
/// </summary>
public enum DroneState
{
    Unknown, // Deckt alle Zustände ab, die nicht definiert sind. Muss kein Fehlerzustand sein.
    Standby,
    Hovering,
    TakingOff,
    Landing,
    MovingForward,
    MovingBackward,
    MovingLeft,
    MovingRight,
    RotatingClockwise,
    RotatingCounterClockwise,
    Rising,
    Sinking,
}