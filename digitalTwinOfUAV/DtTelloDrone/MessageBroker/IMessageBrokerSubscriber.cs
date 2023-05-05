using DtTelloDrone.RyzeSDK.CommunicationInferfaces;

namespace DtTelloDrone.MessageBroker;

public interface IMessageBrokerSubscriber
{
    /// <summary>
    /// Publish a message.
    /// </summary>
    public void PublishMessage(DroneMessage message);
}