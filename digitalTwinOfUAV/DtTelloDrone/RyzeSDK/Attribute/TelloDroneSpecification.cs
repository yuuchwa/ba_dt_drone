namespace DtTelloDrone.RyzeSDK.Attribute;

public static class TelloDroneSpecification
{
    public const float Length = 980;    // Millimeter
    public const float Width = 925;     // Millimeter
    public const float Height = 41;     // Millimeter

    public const int Weight = 80; // Gramm

    public const int maxFlightDuration = 13; // Minutes
    public const int MaxSpeed = 8000; // Millimeter / Seconds
    public const int MaxAltitude = 30000; // Millimeter
    public const int MaxrangeFromRemoteControl = 100000; // Millimeter
}