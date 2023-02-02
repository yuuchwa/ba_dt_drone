using System.Net;

namespace RyzeTelloSDK.Core
{
    public class TelloSettings
    {
        public IPAddress Address { get; set; } = IPAddress.Parse("192.168.10.1");
        public int CommandPort { get; set; } = 8889;
        public int StatePort { get; set; } = 8890;
        public int VideoPort { get; set; } = 11111;
    }
}
