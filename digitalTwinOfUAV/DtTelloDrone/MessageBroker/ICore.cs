using DtTelloDrone.Model.Agent;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;

namespace DtTelloDrone.RyzeSDK;

public interface ICore
{
    /// <summary>
    /// Query a command to the drone.
    /// </summary>
    /// <param name="command"></param>
    public void QueryCommand(DroneCommand command);
    
    /// <summary>
    /// Get the state of the drone.
    /// </summary>
    /// <returns>The state of the drone</returns>
    public TelloStateParameter GetStateParameter();

    public void Subscribe(ICoreSubscriber subscriber);

    /// <summary>
    /// Close and dispose.
    /// </summary>
    public void Close();
}