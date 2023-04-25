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
    MovingForwards,
    MovingBackwards,
    MovingLeft,
    MovingRight,
    RotatingClockwise,
    RotatingCounterClockwise,
    Rising,
    Sinking,
}

/*
https://stackoverflow.com/questions/2109969/c-is-there-a-way-to-classify-enums
 Soll dazu dienen die Abfrage der passenden Zustände zu vereinfachen.
public class DroneState
{
    public enum HorizontalMovement
    {
        Hovering,
        MovingForwards,
        MovingBackwards,
        MovingLeft,
        MovingRight,
    }

    public enum VerticalMovement
    {
        Sinking,
        Rising
    }

    public enum Rotationg
    {
        RotatingClockwise,
        RotatingCounterClockwise,
    }

    public enum Unknown
    {
        Unknown
    }   
}*/