namespace DtTelloDrone.MessageBroker;

/// <summary>
/// This enum lists all the possible senders of a DroneMessage object.
/// </summary>
public enum MessageSender
{
    RemoteControl,
    DigitalTwin,
    Drone,
}