using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.Logger;
using DtTelloDrone.RyzeSDK.Models;

namespace DtTelloDrone.RyzeSDK.Core
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
        /// The main loop
        /// </summary>
        private Task _mainLoop;
        
        /// <summary>
        /// Token for terminating a thread.
        /// </summary>
        private CancellationTokenSource _cancellationToken;
        
        public event Action<Exception> OnException;
        public event Action<string> OnStateRaw;
        public event Action<TelloStateParameter> OnState;
        
        /// <summary>
        /// Instantiates the TelloStateServer.
        /// </summary>
        /// <param name="connectionSettings">The settings for Tello.</param>
        public TelloStateServer()
        {
            udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, TelloSettings.StateUdpPort));
            udpServer.Client.ReceiveTimeout = 3000;
        }

        /// <summary>
        /// Terminate the thread.
        /// </summary>
        public void Close()
        {
            _cancellationToken.Cancel();
        }

        /// <summary>
        /// Starts a async operation for listing to the socket.
        /// </summary>
        public void Listen()
        {
            _cancellationToken = new CancellationTokenSource();
            _mainLoop = Task.Run(ListenTask, _cancellationToken.Token);
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
                    var data = Encoding.ASCII.GetString(result.Buffer).Replace('\n', ' ');
                    
Console.Write(data);
                    OnStateRaw?.Invoke(data);
                    OnState?.Invoke(TelloStateParameter.FromString(data));
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
