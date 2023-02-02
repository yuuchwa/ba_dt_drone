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

        public static Task<bool> RC(this ITelloClient tello, int leftright, int forwardbackward, int updown, int yaw)
        {
            CommandConstraints.CheckRC(leftright);
            CommandConstraints.CheckRC(forwardbackward);
            CommandConstraints.CheckRC(updown);
            CommandConstraints.CheckRC(yaw);
            return tello.SendAction($"rc {leftright} {forwardbackward} {updown} {yaw}");
        }

        public static Task<bool> WiFi(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendAction($"wifi {ssid} {pass}");
        }

        // ToDo: set mon
        // ToDo: set moff
        // ToDo: set mdirection x

        public static Task<bool> Station(this ITelloClient tello, string ssid, string pass)
        {
            return tello.SendAction($"ap {ssid} {pass}");
        }
    }
}
