using DtTelloDrone.RyzeSDK.Models;
using NetTopologySuite.Operation.Valid;

namespace DtTelloDrone.Model.Services;

public static class TelloFlightMetrics
{
    public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    // TODO: constanten umbenennen
    public const int MinInvalidTof = -1;
    public const int ValidNonMeasureableTof = 10; // Wert für jede ungültige Messung oder eine Flughöhe unter 30 cm werden.
    public const int StartMeasureableTof = 30; // Ab 30 cm aufwärts kann die ToF Messung erfasst werden
    public const int MaxHeight = 10000;
    
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
    public const int InitialYaw = 0;
    public const int MinYawDegree = -179;
    public const int MaxYawDegree = 179;

    public const int OverheatingThreshold = 80; // Degree Celsius

    public const int BatteryLowThreshold = 15;

    public static bool ValidateParameters(TelloStateParameter parameters)
    {
        return ValidateYaw(parameters.Yaw) &&
               ValidateRoll(parameters.Roll) &&
               ValidatePitch(parameters.Pitch) && 
               ValidateTof(parameters.TOF) &&
               ValidateHeight(parameters.Height) &&
               ValidateVelocityX(parameters.VelocityX) &&
               ValidateVelocityY(parameters.VelocityY) &&
               ValidateVelocityZ(parameters.VelocityZ);
    }

    public static bool IsOverheating(int temperature)
    {
        return OverheatingThreshold < temperature;
    }

    public static bool BatteryLow(int battery)
    {
        return battery < BatteryLowThreshold;
    }
    
    private static bool ValidateYaw(double yaw)
    {
        bool isValid = MinYawDegree <= yaw && yaw <= MaxYawDegree;

        if (!isValid)
        {
            Logger.Error($"Yaw value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidateRoll(double roll)
    {
        bool isValid = MinRollDegree <= roll && roll <= MaxRollDegree;

        if (!isValid)
        {
            Logger.Error($"Roll value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidatePitch(double pitch)
    {
        bool isValid = MinPitchDegree <= pitch && pitch <= MaxPitchDegree;

        if (!isValid)
        {
            Logger.Error($"Pitch value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidateVelocityX(double velocity)
    {
        bool isValid = MinXVelocity <= velocity && velocity <= MaxXVelocity;

        if (!isValid)
        {
            Logger.Error($"VelocityX value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidateVelocityY(double velocity)
    {
        bool isValid = MinYVelocity <= velocity && velocity <= MaxYVelocity;

        if (!isValid)
        {
            Logger.Error($"VelocityY value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidateVelocityZ(double velocity)
    {
        bool isValid = MinZVelocity <= velocity && velocity <= MaxZVelocity;

        if (!isValid)
        {
            Logger.Error($"VelocityZ value is invalid");
        }

        return isValid;
    }
    
    private static bool ValidateTof(double tof)
    {
        bool isValid = !(tof <= MinInvalidTof) && (tof == ValidNonMeasureableTof || StartMeasureableTof <= tof);

        if (!isValid)
        {
            Logger.Error($"Tof value is invalid");
        }

        return isValid;
    }

    private static bool ValidateHeight(int height)
    {
        return height <= MaxHeight;
    }
}