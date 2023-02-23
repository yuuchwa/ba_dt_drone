using System.Net;

namespace RyzeTelloSDK.Core
{
    /// <summary>
    /// Class that contains all configuration settings.
    /// </summary>
    public static class TelloSettings
    {
        /// <summary>
        ///  Ip Adress to reach the Tello drone.
        /// </summary>
        public const string IpAddress = "192.168.10.1";
        //public IPAddress Address { get; } = IPAddress.Parse("192.168.10.1");

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
        
        /// <summary>
        /// Time until thread abort waiting for a reponse.
        /// </summary>
        public const int ResponseTimeOut = 7000; // Milliseconds
        
        /// <summary>
        /// Time between next command.
        /// </summary>
        public const double TimeBetweenCommand = 100; // Seconds
        
        /// <summary>
        /// Number of retrys for sending a upd command before abort action.
        /// </summary>
        public const int RetryCount = 3;
    }
}
