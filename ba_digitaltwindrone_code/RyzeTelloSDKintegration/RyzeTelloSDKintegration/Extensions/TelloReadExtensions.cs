using RyzeTelloSDK.Core;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Extensions
{
    public static class TelloReadExtensions
    {
        public static Task<string> GetSpeed(this ITelloClient tello)
        {
            return tello.SendCommand("speed?");
        }

        public static Task<string> GetBattery(this ITelloClient tello)
        {
            return tello.SendCommand("battery?");
        }

        public static Task<string> GetTime(this ITelloClient tello)
        {
            return tello.SendCommand("time?");
        }

        public static Task<string> GetWifi(this ITelloClient tello)
        {
            return tello.SendCommand("wifi?");
        }

        public static Task<string> GetSDK(this ITelloClient tello)
        {
            return tello.SendCommand("sdk?");
        }

        public static Task<string> GetSN(this ITelloClient tello)
        {
            return tello.SendCommand("sn?");
        }
    }
}
