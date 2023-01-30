using RyzeTelloSDK.Core;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Extensions
{
    public static class TelloSetExtensions
    {
        public static Task<bool> Speed(this ITelloClient tello, int speed)
        {
            CommandConstraints.CheckSpeed(speed);
            return tello.SendAction($"speed {speed}");
        }

        public static Task<bool> RemoteControll(this ITelloClient tello, int leftright, int forwardbackward, int updown, int yaw)
        {
            CommandConstraints.CheckRemoteContoll(leftright);
            CommandConstraints.CheckRemoteContoll(forwardbackward);
            CommandConstraints.CheckRemoteContoll(updown);
            CommandConstraints.CheckRemoteContoll(yaw);
            return tello.SendAction($"rc {leftright} {forwardbackward} {updown} {yaw}");
        }

        public static Task<bool> WiFi(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendAction($"wifi {ssid} {pass}");
        }

        // Commands for controlling drone with Mission  pads
        
        // ToDo: set mon
        // ToDo: set moff
        // ToDo: set mdirection x

        public static Task<bool> Station(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendAction($"ap {ssid} {pass}");
        }
    }
}
