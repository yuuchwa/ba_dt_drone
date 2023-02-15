using RyzeTelloSDK.Enum;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace RyzeTelloSDK.Core
{
    public class TelloClient : ITelloClient
    {
        private readonly UdpClient udpClient;
        private readonly TelloSettings telloSettings;

        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        public bool IsConnected() => udpClient.Client != null && udpClient.Client.Connected;

        public TelloClient(TelloSettings settings)
        {
            telloSettings = settings;
            udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 3000; // ToDo: might wanna lower it / move it to settings
        }

        public TelloClient(TelloSettings settings, UdpClient client)
        {
            telloSettings = settings;
            udpClient = client;
        }

        public void Connect()
        {
            udpClient.Connect(new IPEndPoint(telloSettings.Address, telloSettings.CommandPort));
        }

        public void Disconnect()
        {
            udpClient.Close();
        }

        public async Task<string> SendCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentException("Empty command", nameof(command));
            if (!IsConnected()) throw new Exception("Not connected to Tello");

            var buf = Encoding.ASCII.GetBytes(command);
            await udpClient.SendAsync(buf, buf.Length);

            if (command.StartsWith("rc")) return "ok"; // ignore response so there's no delay in rc commands; ToDo: find a better way to handle this
            var response = udpClient.Receive(ref endPoint);
            return Encoding.ASCII.GetString(response);
        }

        public async Task<bool> SendAction(string command)
        {
            var response = await SendCommand(command);
            Console.WriteLine(response);
            switch(response.ToLower())
            {
                case "ok":
                    return true;
                case "false":
                    return false;
                default:
                    throw new Exception($"Unknown response: {response}");
            }
        }

        public void Dispose() => udpClient.Dispose();
    }
}
