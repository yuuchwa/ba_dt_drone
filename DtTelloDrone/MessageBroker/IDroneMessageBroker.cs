using DtTelloDrone.TelloSdk.DataModels;

namespace DtTelloDrone.MessageBroker;

/// <summary>
/// The IDroneMessageBroker interface defines the methods for a drone message broker,
/// which is responsible for handling messages sent between different components of a drone system. 
/// </summary>
public interface IDroneMessageBroker
{
    /// <summary>
    /// This method querys a message to the message broker.
    /// </summary>
    /// <param name="message">The Message to be queried</param>
    public void QueryMessage(DroneMessage message);
    
    /// <summary>
    /// This method return the latest state of the drone.
    /// </summary>
    /// <returns>The state of the drone.</returns>
    public TelloStateParameter GetStateParameter();

    /// <summary>
    /// Check the status whether the message broker is connected to the drone.
    /// </summary>
    /// <returns>If true, the they are connected.</returns>
    public bool DroneConnected();

    /// <summary>
    /// Check whether the drone is set to SDK mode.
    /// </summary>
    /// <returns>true if set</returns>
    public bool DroneIsInCommandMode();

    public void Subscribe(IMessageBrokerSubscriber subscriber);

    /// <summary>
    /// Close and dispose the instance.
    /// </summary>
    public void Close();
}