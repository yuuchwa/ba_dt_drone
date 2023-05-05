using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Agent;
using DtTelloDrone.RyzeSDK.CommunicationInferfaces;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;

namespace DtTelloDrone.RyzeSDK;

public interface IDroneMessageBroker
{
    /// <summary>
    /// Query a message to the Message broker.
    /// </summary>
    /// <param name="message">The Message</param>
    public void QueryMessage(DroneMessage message);
    
    /// <summary>
    /// Get the state of the drone.
    /// </summary>
    /// <returns>The state of the drone</returns>
    public TelloStateParameter GetStateParameter();

    public void Subscribe(IMessageBrokerSubscriber subscriber);

    /// <summary>
    /// Close and dispose.
    /// </summary>
    public void Close();
}