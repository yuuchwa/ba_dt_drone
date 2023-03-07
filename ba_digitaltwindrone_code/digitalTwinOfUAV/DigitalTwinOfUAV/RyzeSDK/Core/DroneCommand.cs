namespace RyzeTelloSDK.Enum;

public class DroneCommand
{
    public TelloAction _action { get; private set; } // TelloAction generalisieren

    public int _value { get; private set; }

    public DroneCommand(TelloAction action, int value)
    {
        if (action != null)
        {
            
        }

        _action = action;
        _value = value;
    }
}