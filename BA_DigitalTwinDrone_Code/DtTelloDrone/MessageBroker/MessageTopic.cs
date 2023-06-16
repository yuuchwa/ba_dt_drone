namespace DtTelloDrone.MessageBroker;

/// <summary>
/// This enum represents the different topics of drone messages that can be handled.
/// </summary>
public enum MessageTopic
{
    DroneCommand,
    StatusResponse,
    Operation,
    Unknown
}