using RyzeTelloSDK.Core;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Extensions
{
    /// <summary>
    /// Extends the Core Class with configuration instructions. 
    /// </summary>
    public static class TelloSetExtensions
    {
        /// <summary>
        /// Sets the speed of the Tello drone.
        /// </summary>
        /// <param name="tello">The Udp client.</param>
        /// <param name="speed">The speed.</param>
        /// <returns>Success Status</returns>
        public static Task<bool> Speed(this ITelloClient tello, int speed)
        {
            CommandConstraints.CheckSpeed(speed);
            return tello.SendCommandWithResponse($"speed {speed}");
        }

        /// <summary>
        /// Configurate the remote control.
        /// </summary>
        /// <param name="tello">The Udp client.</param>
        /// <param name="leftright"></param>
        /// <param name="forwardbackward"></param>
        /// <param name="updown"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public static Task<bool> RemoteControl(this ITelloClient tello, int leftright, int forwardbackward, int updown, int yaw)
        {
            CommandConstraints.CheckRC(leftright);
            CommandConstraints.CheckRC(forwardbackward);
            CommandConstraints.CheckRC(updown);
            CommandConstraints.CheckRC(yaw);
            return tello.SendCommandWithResponse($"rc {leftright} {forwardbackward} {updown} {yaw}");
        }

        /// <summary>
        /// Set the Wifi name and Passwort.
        /// </summary>
        /// <param name="tello">The Udp client.</param>
        /// <param name="ssid">The WiFi name.</param>
        /// <param name="pass">The WiFi password.</param>
        /// <returns></returns>
        public static Task<bool> WiFi(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendCommandWithResponse($"wifi {ssid} {pass}");
        }

        // ToDo: set mon
        // ToDo: set moff
        // ToDo: set mdirection x
        public static Task<bool> Station(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendCommandWithResponse($"ap {ssid} {pass}");
        }
    }
}
