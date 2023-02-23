using System.Net;

namespace RyzeTelloSDK.Core
{
    /// <summary>
    /// Class that contains all configuration settings.
    /// </summary>
    public class TelloSettings
    {
    	/// <summary>
        ///  Ip Adress to reach the Tello drone.
        /// </summary>
        public IPAddress Address { get; } = IPAddress.Parse("192.168.10.1");

        /// <summary>
        /// Port for sending commands.
        /// </summary>
        public const int CommandUdpPort = 8889;

        /// <summary>
        /// Port for receiving state information.
        /// </summary>
        public const int StateUdpPort = 8890;

        /// <summary>
        /// Port for receiving video stream data.
        /// </summary>
        public const int VideoStreamPort = 11111;
    }
}
