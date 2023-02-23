using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using RyzeTelloSDK.Core;
using System.Linq;

namespace RyzeTelloSDKintegration.Core
{
    public class TelloClient : ITelloClient
    {
        private const string SuccessResponse = "ok";
        private const string FailedResponse = "failed";
        
        private readonly UdpClient _udpClient;
        private readonly IPAddress _ipAddress;
        private IPEndPoint _endPoint;

        private readonly Thread _responseListener;
        private readonly ConcurrentQueue<string> _responses;
        private long _lastReceivedCommandTs;


        public bool IsConnected() => _udpClient.Client.Connected;

        /// <summary>
        /// Instantiate the Tello client.
        /// </summary>
        public TelloClient()
        {
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveTimeout = TelloSettings.ResponseTimeOut;
            _ipAddress = IPAddress.Parse(TelloSettings.IpAddress);
            _endPoint = new IPEndPoint(_ipAddress, 0);
            Connect();
            
            _responses = new ConcurrentQueue<string>();
            _responseListener = new Thread(ResponseListener);
            //_responseListener.Start();

            _lastReceivedCommandTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void Connect()
        {
            _udpClient.Connect(new IPEndPoint(_ipAddress, TelloSettings.CommandUdpPort));
        }

        public void Disconnect()
        {
            _responses.Clear();
            _udpClient.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> SendCommand(string command)
        {
            // Überprüft, ob das Argument leer ist.
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Empty command", nameof(command));
            }
            
            // Überprüft, ob die Verbindung noch steht.
            if (!IsConnected())
            {
                throw new Exception("Not connected to Tello");
            }

            // Überprüft die Dauer seit dem letzten Command.
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long timeDifferenceSinceLastCommand = now - _lastReceivedCommandTs;

            if (timeDifferenceSinceLastCommand < TelloSettings.TimeBetweenCommand)
            {
                Thread.Sleep((int)timeDifferenceSinceLastCommand);
            }
            
            _lastReceivedCommandTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            byte[] datagram = Encoding.ASCII.GetBytes(command);
            await _udpClient.SendAsync(datagram, datagram.Length); // wartet nur drauf, bis die Nachricht tatsächlich abgeschickt wurde.
            
            if (command.StartsWith("rc")) return "ok"; // ignore response so there's no delay in rc commands; ToDo: find a better way to handle this
            
            string response = null;
            var ans = _udpClient.Receive(ref _endPoint);
            response = Encoding.ASCII.GetString(ans);
            
            /*
             Für den Client ist ein zusätzlicher Thread erstmal nicht notwendig.
            while (!_responses.TryDequeue(out response))
            {
                // _responses.TryDequeue(out response);
            }
            */

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> SendCommandWithResponse(string command)
        {
            for (int attempt = 0; attempt < TelloSettings.RetryCount; attempt++)
            {
                var response = await SendCommand(command);
                
                if (response != null && String.CompareOrdinal(response.ToLower(), SuccessResponse) == 0)
                {
                    Console.WriteLine($"Command '{command}' was successful");
                    return true;
                }
                // Logging
                Console.WriteLine($"Command attempt {attempt} failed for command: '{command}'");
            }
            
            return false;
        }

        private void ResponseListener()
        {
            while (IsConnected())
            {
                var response = _udpClient.Receive(ref _endPoint);

                if (response.Length != 0)
                {
                    _responses.Enqueue(Encoding.ASCII.GetString(response));
                }
            }   

        }

        public void Dispose() => _udpClient.Dispose();
    }
}
