namespace DtTelloDrone.MessageBroker;

/// <summary>
/// This interface describes all subscriber types that participate in the publish-subscribe communication with the message broker.
/// /// </summary>
public interface IMessageBrokerSubscriber
{
    /// <summary>
    /// Publish a message to the subscriber.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    public void PublishMessage(DroneMessage message);
}