using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalTwinOfATelloDrone.RyzeSDK.Core
{
    /// <summary>
    /// Class for receiving the video data stream form the tello.
    /// </summary>
    public class TelloVideoServer : ITelloServer
    {
        private readonly UdpClient _udpServer;

        private Task mainLoop;
        private CancellationTokenSource cts;

        public event Action<Exception> OnException;
        public event Action<byte[]> OnData;

        public TelloVideoServer()
        {
            IPAddress ipAddress = IPAddress.Parse(TelloSettings.IpAddress);
            _udpServer = new UdpClient(new IPEndPoint(ipAddress, TelloSettings.VideoStreamPort));
            _udpServer.Client.ReceiveTimeout = 3000;
        }

        public void Close()
        {
            cts.Cancel();
        }

        public void Listen()
        {
            cts = new CancellationTokenSource();
            mainLoop = Task.Run(ListenTask, cts.Token);
        }

        private async void ListenTask()
        {
            while (true)
            {
                try
                {
                    var result = await _udpServer.ReceiveAsync();
                    OnData?.Invoke(result.Buffer);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                }
            }
        }

        public void Dispose() => _udpServer.Dispose();
    }
}
