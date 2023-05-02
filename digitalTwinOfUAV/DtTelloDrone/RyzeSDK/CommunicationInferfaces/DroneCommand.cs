using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.RyzeSDK.Core;

public class DroneCommand
{
    public TelloAction _action { get; set; } // TelloAction generalisieren
    public int _value { get; set; }

    public DroneCommand(TelloAction action, int value)
    {
        _action = action;
        _value = value;
    }
}