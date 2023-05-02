using ServiceStack.Redis;

namespace DtTelloDrone.MessageBroker;

public enum TelloTopic
{
    DroneControl,
    StatusResponse,
    Operation,
    Unknown
}