using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.TelloSdk.DataModels;

namespace DtTelloDrone.TelloSdk.CommunicationInferfaces
{
    /// <summary>
    /// The class for receiving the state information of the tello drone.
    /// </summary>
    public class TelloStateServer : IDroneServer
    {
    	/// <summary>
        /// The UPD client
        /// </summary>
        private readonly UdpClient udpServer;
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

        private string _rawdata;
        private TelloStateParameter _stateData;
        
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

        public string GetRawState()
        {
            return _rawdata;
        }

        public TelloStateParameter GetStateParameter()
        {
            return _stateData;
        }

        /// <summary>
        /// Listing on upd socket.
        /// </summary>
        private async void ListenTask()
        {
            Logger.Trace("Pitch;Roll;Yaw;VelocityX;VelocityY;VelocityZ;TemperaturLow;TemperaturHigh;TimeOfFlight;Height;Battery;Barometer;Time;AccelerationX;AccelerationY;AccelerationZ;Timestamp");
            while (true)
            {
                try
                {
                    var result = await udpServer.ReceiveAsync();
                    var data = Encoding.ASCII.GetString(result.Buffer).Replace('\n', ' ');
                    _rawdata = data;
                    _stateData = TelloStateParameter.FromString(data);

                    Logger.Trace(_stateData.ConvertToCsv());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public void Dispose() => udpServer.Dispose();
    }
}
