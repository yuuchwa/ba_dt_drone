namespace DtTelloDrone.Model.Agent;

public interface ICoreSubscriber
{
    /// <summary>
    /// Publish a message.
    /// </summary>
    public void PublishMessage(CoreMessage message);
}