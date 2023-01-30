using RyzeTelloSDK.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfUAV.TelloSDK.Core;

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
        private readonly UdpClient _udpClient;
        
        /// <summary>
        /// The setting for the tello drone.
        /// </summary>
        private readonly TelloConnectionSettings _telloConnectionSettings;

        /// <summary>
        /// The main loop
        /// </summary>
        private Task mainLoop;
        
        /// <summary>
        /// Token for canceling a thread.
        /// </summary>
        private CancellationTokenSource _cts;

        public event Action<Exception> OnException;
        public event Action<string> OnStateRaw;
        public event Action<TelloState> OnState;

        #region Constructor
        
        /// <summary>
        /// Instantiates the TelloStateServer.
        /// </summary>
        /// <param name="connectionSettings">The settings for Tello.</param>
        public TelloStateServer(TelloConnectionSettings connectionSettings)
        {
            _telloConnectionSettings = connectionSettings;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _telloConnectionSettings.StatePort));
            _udpClient.Client.ReceiveTimeout = 3000;
        }  

        #endregion
        
        /// <summary>
        /// Terminate the thread.
        /// </summary>
        public void Close()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// Starts a async operation for listing to the socket.
        /// </summary>
        public void Listen()
        {
            _cts = new CancellationTokenSource();
            mainLoop = Task.Run(ListenTask, _cts.Token);
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
                    var result = await _udpClient.ReceiveAsync();
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

        public void Dispose() => _udpClient.Dispose();
    }
}
