namespace DtTelloDrone.Model.Attributes;

/// <summary>
/// This enum represent the possible states of the drone.
/// </summary>
public enum DroneState
{
    Unknown, // this value is used to represent all states which are not covered yet.
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