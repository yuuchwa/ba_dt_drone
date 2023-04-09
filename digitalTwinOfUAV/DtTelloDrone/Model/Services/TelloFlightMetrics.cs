namespace DtTelloDrone.Model.Services;

public static class TelloFlightMetrics
{
    // TODO: constanten umbenennen
    public const int NotMeasureableTof = 10; // Wert für jede ungültige Messung oder eine Flughöhe unter 30 cm werden.
    public const int LowerMeasureableTof = 30; // Ab 30 cm aufwärts kann die ToF Messung erfasst werden
    public const int MaxMeasurebaleTof = 3000; // Name falsch, Max Höhe gegebenfalls noch nicht erreicht. wurde aber noch nicht überprüft.
    
    // Acceleration
    public const int LowerXAccelerationForHovering = -50;
    public const int UpperXAccelerationForHovering = 50;

    public const int LowerYAccelerationForHovering = -50;
    public const int UpperYAccelerationForHovering = 50;
    
    public const int LowerZAccelerationForHovering = -1100;
    public const int UpperZAccelerationForHovering = -950;
    
    //Velocity
    public const int RestVelocity = 0;
    
    // x < 0 -> Backward
    // 0 < x -> Forward
    public const int MinXVelocity = -30;
    public const int MaxXVelocity = 30;
    
    // y < 0 -> Right
    // 0 < y -> Left
    public const int MinYVelocity = -30;
    public const int MaxYVelocity = 30;

    // z < 0 -> Down
    // 0 < z -> Up
    public const int MinZVelocity = -30;
    public const int MaxZVelocity = 30;

    // Roll
    // 1 <= x -> Right
    // x <= -1 -> Left
    public const int RollBalanced = 0;
    public const int MinRollDegree = -179;
    public const int MaxRollDegree = 179;

    // Pitch
    // 1 <= x -> Backward
    // x <= -1 -> Forward
    public const int PitchBalanced = 0;
    public const int MinPitchDegree = -179;
    public const int MaxPitchDegree = 179;
    
    // Yaw
    // x moving to -1799 CCW
    // x moving to 1799 CW 
    public const int Yawbalanced = 0;
    public const int MinYawDegree = -179;
    public const int MaxYawDegree = 179;
}