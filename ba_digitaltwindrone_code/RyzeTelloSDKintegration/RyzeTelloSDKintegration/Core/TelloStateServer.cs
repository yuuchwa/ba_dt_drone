using RyzeTelloSDK.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Core
{
    public class TelloStateServer : ITelloServer
    {
        private readonly UdpClient udpServer;
        private readonly TelloSettings telloSettings;

        private Task mainLoop;
        private CancellationTokenSource cts;

        public event Action<Exception> OnException;
        public event Action<string> OnStateRaw;
        public event Action<TelloState> OnState;

        public TelloStateServer(TelloSettings settings)
        {
            telloSettings = settings;
            udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, telloSettings.StatePort));
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
                    var data = Encoding.ASCII.GetString(result.Buffer);
                    OnStateRaw?.Invoke(data);
                    OnState?.Invoke(TelloState.FromString(data));
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
