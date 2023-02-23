using RyzeTelloSDK.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Core
{
    /// <summary>
    /// The class for receiving the state information of the tello drone.
    /// </summary>
    public class TelloStateServer : ITelloServer
    {
    	/// <summary>
        /// The UPD client
        /// </summary>
        private readonly UdpClient udpServer;
        
        /// <summary>
        /// The setting for the tello drone.
        /// </summary>
        private readonly TelloSettings telloSettings;

	/// <summary>
        /// The main loop
        /// </summary>
        private Task mainLoop;
        
        /// <summary>
        /// Token for canceling a thread.
        /// </summary>
        private CancellationTokenSource cts;

        public event Action<Exception> OnException;
        public event Action<string> OnStateRaw;
        public event Action<TelloState> OnState;
        
        /// <summary>
        /// Instantiates the TelloStateServer.
        /// </summary>
        /// <param name="connectionSettings">The settings for Tello.</param>
        public TelloStateServer(TelloSettings settings)
        {
            telloSettings = settings;
            udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, TelloSettings.StateUdpPort));
            udpServer.Client.ReceiveTimeout = 3000;
        }

        /// <summary>
        /// Terminate the thread.
        /// </summary>
        public void Close()
        {
            cts.Cancel();
        }

        /// <summary>
        /// Starts a async operation for listing to the socket.
        /// </summary>
        public void Listen()
        {
            cts = new CancellationTokenSource();
            mainLoop = Task.Run(ListenTask, cts.Token);
        }

        /// <summary>
        /// Listing on upd socket.
        /// </summary>
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
