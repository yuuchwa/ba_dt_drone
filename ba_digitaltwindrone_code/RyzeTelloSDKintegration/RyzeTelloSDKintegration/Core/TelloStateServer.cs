using RyzeTelloSDK.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
        /// The main loop
        /// </summary>
        private Task _mainLoop;
        
        /// <summary>
        /// Token for canceling a thread.
        /// </summary>
        private CancellationTokenSource _cancellationToken;

        public event Action<Exception> OnException;
        public event Action<string> OnStateRaw;
        public event Action<TelloState> OnState;
        
        /// <summary>
        /// Instantiates the TelloStateServer.
        /// </summary>
        /// <param name="connectionSettings">The settings for Tello.</param>
        public TelloStateServer()
        {
            udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, TelloSettings.StateUdpPort));
            udpServer.Client.ReceiveTimeout = 3000;
            Console.WriteLine("Stateserver Instantiated");
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
            Console.WriteLine("started");

            while (true)
            {
                try
                {
                    Console.WriteLine("warte");
                    var result = await udpServer.ReceiveAsync();
                    var data = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine("komme rein");

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
