using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Core
{
    public class TelloVideoServer : ITelloServer
    {
        private readonly UdpClient udpServer;
        private readonly TelloSettings telloSettings;

        private Task mainLoop;
        private CancellationTokenSource cts;

        public event Action<Exception> OnException;
        public event Action<byte[]> OnData;

        public TelloVideoServer(TelloSettings settings)
        {
            telloSettings = settings;
            udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, TelloSettings.VideoStreamPort));
            udpServer.Client.ReceiveTimeout = 3000;
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
                    var result = await udpServer.ReceiveAsync();
                    OnData?.Invoke(result.Buffer);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                }
            }
        }

        public void Dispose() => udpServer.Dispose();
    }
}
