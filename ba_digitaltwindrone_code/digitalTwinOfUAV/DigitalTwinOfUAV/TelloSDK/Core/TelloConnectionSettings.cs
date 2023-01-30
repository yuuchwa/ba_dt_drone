using System.Net;

namespace DigitalTwinOfUAV.TelloSDK.Core
{
    public class TelloConnectionSettings
    {
        /// <summary>
        ///  Ip Adress to reach the Tello drone.
        /// </summary>
        public IPAddress IpAddress { get; set; } = IPAddress.Parse("192.168.10.1");
        
        /// <summary>
        /// Port for sending commands.
        /// </summary>
        public int CommandPort { get; set; } = 8889;
        
        /// <summary>
        /// Port for receiving state information.
        /// </summary>
        public int StatePort { get; set; } = 8890;
        
        /// <summary>
        /// Port for receiving video stream data.
        /// </summary>
        public int VideoStreamPort { get; set; } = 11111;
    }
}
