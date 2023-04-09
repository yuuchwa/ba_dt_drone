using DigitalTwinOfATelloDrone.RyzeSDK.Core;
using DigitalTwinOfATelloDrone.RyzeSDK.Models;

namespace DigitalTwinOfATelloDrone.RyzeSDK;

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
}