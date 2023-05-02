namespace DtTelloDrone.Model.Agent;

public interface IMessageBrokerSubscriber
{
    /// <summary>
    /// Publish a message.
    /// </summary>
    public void PublishMessage(MessageBrokerMessage messageBrokerMessage);
}