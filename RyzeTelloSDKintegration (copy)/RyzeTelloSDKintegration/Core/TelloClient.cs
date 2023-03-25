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
        private const double TimeBetweenCommand = 0.1; // Seconds
        private const byte ResponseTimeOut = 7; // Seconds
        private const byte RetryCount = 3;
        
        private readonly UdpClient _udpClient;
        private readonly TelloSettings _telloSettings;

        private IPEndPoint _endPoint = new IPEndPoint(IPAddress.Any, 0);

        public bool IsConnected() => _udpClient.Client != null && _udpClient.Client.Connected;

        public TelloClient(TelloSettings settings)
        {
            _telloSettings = settings;
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveTimeout = 3000; // ToDo: might wanna lower it / move it to settings
        }

        public TelloClient(TelloSettings settings, UdpClient client)
        {
            _telloSettings = settings;
            _udpClient = client;
        }

        public void Connect()
        {
            _udpClient.Connect(new IPEndPoint(_telloSettings.Address, TelloSettings.CommandUdpPort));
        }

        public void Disconnect()
        {
            _udpClient.Close();
        }

        public async Task<string> SendCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentException("Empty command", nameof(command));
            if (!IsConnected()) throw new Exception("Not connected to Tello");

            var buf = Encoding.ASCII.GetBytes(command);
            await _udpClient.SendAsync(buf, buf.Length);

            if (command.StartsWith("rc")) return "ok"; // ignore response so there's no delay in rc commands; ToDo: find a better way to handle this
            var response = _udpClient.Receive(ref _endPoint);
            return Encoding.ASCII.GetString(response);
        }

        public async Task<bool> SendAction(string command)
        {
            var response = await SendCommand(command);
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

        public void Dispose() => _udpClient.Dispose();
    }
}
