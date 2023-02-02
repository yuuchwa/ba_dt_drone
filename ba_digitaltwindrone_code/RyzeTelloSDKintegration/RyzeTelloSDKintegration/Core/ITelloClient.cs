using System;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Core
{
    /// <summary>
    /// Interface for tello command actions.
    /// </summary>
    public interface ITelloClient : IDisposable
    {
        bool IsConnected();

        void Connect();
        void Disconnect();

        /// <summary>
        /// Send command to drone
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Drone response message.</returns>
        Task<string> SendCommand(string command);
        
        /// <summary>
        /// Send action command to drone
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Drone response message.</returns>
        Task<bool> SendAction(string command);
    }
}
