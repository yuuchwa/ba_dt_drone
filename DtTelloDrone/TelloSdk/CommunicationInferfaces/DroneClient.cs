using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;

namespace DtTelloDrone.TelloSdk.CommunicationInferfaces
{
    public class DroneClient : IDroneClient
    {
        private const string SuccessResponse = "ok";
        private const string FailedResponse = "failed";
        
        private readonly UdpClient _udpClient;
        private readonly IPAddress _ipAddress;
        private IPEndPoint _endPoint;

        private readonly Thread _responseListener;
        private readonly ConcurrentQueue<string> _responses;
        private long _lastReceivedCommandTs;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const int Idle = 0;
        
        public bool IsConnected() => _udpClient.Client.Connected;

        /// <summary>
        /// Instantiate the Tello client.
        /// </summary>
        public DroneClient()
        {
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveTimeout = TelloSettings.ResponseTimeOut;
            _ipAddress = IPAddress.Parse(TelloSettings.IpAddress);
            _endPoint = new IPEndPoint(_ipAddress, 0);
            
            _responses = new ConcurrentQueue<string>();
            _responseListener = new Thread(ResponseListener);
            _lastReceivedCommandTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void Connect()
        {
            Logger.Info($"Message Broker listening on {TelloSettings.CommandUdpPort}");
            _udpClient.Connect(new IPEndPoint(_ipAddress, TelloSettings.CommandUdpPort));
        }

        public void Disconnect()
        {
            _responses.Clear();
            _udpClient.Close();
        }

        public void Dispose() => _udpClient.Dispose();
        
        public bool CommandModeEnabled()
        {
            try
            {
                Fly(MoveDirection.Stop, 0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DroneIsActive()
        {
            try
            {
                GetBattery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region TelloControlExtensions

        /// <summary>
        /// Enable Tello to receive messages.
        /// </summary>
        /// <returns>The response.</returns>
        public Task<bool> EnableCommandMode()
        {
            return SendCommandWithResponse("command");
        }

        /// <summary>
        /// Disable Tello to receive messages.
        /// </summary>
        /// <returns>The response</returns>
        public Task<bool> DisableCommandMode()
        {
            return SendCommandWithResponse("command");
        }

        /// <summary>
        /// Command drone to Take off.
        /// </summary>
        /// <returns>The response</returns>
        public Task<bool> TakeOff()
        {
            return SendCommandWithResponse("takeoff");
        }
        
        /// <summary>
        /// Command drone to land.
        /// </summary>
        /// <returns>The response</returns>
        public Task<bool> Land()
        {
            return SendCommandWithResponse("land");
        }

        /// <summary>
        /// Command drone to activate video stream.
        /// </summary>
        /// <returns></returns>
        public Task<bool> StreamOn()
        {
            return SendCommandWithResponse("streamon");
        }

        /// <summary>
        /// Command drone to deactivate video stream.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public Task<bool> StreamOff()
        {
            return SendCommandWithResponse("streamoff");
        }

        /// <summary>
        /// Command drone to land immediately.
        /// </summary>
        /// <param name="telloClient">Upd server connected to tello</param>
        /// <returns></returns>
        public Task<bool> Emergency()
        {
            return SendCommandWithResponse("emergency");
        }

        /// <summary>
        /// Command drone to fly in a specific direciton.
        /// </summary>
        /// <param name="direction">the direction in which the drone should fly.</param>
        /// <param name="speed">The speed in which the drone should fly.s</param>
        /// <returns></returns>
        public void Fly(MoveDirection direction, int speed)
        {
            CommandConstraints.CheckRC(speed);
            string command;

            switch (direction)
            {
                case MoveDirection.Stop:
                    command = $"rc {Idle} {speed} {Idle} {Idle}";
                    break;
                case MoveDirection.Forward:
                    command = $"rc {Idle} {speed} {Idle} {Idle}";
                    break;
                case MoveDirection.Back:
                    speed *= -1;
                    command = $"rc {Idle} {speed} {Idle} {Idle}";
                    break;
                case MoveDirection.Right:
                    command = $"rc {speed} {Idle} {Idle} {Idle}";
                    break;
                case MoveDirection.Left:
                    speed *= -1;
                    command = $"rc {speed} {Idle} {Idle} {Idle}";
                    break;
                case MoveDirection.Rise:
                    command = $"rc {Idle} {Idle} {speed} {Idle}";
                    break;
                case MoveDirection.Sink:
                    speed *= -1;
                    command = $"rc {Idle} {Idle} {speed} {Idle}";
                    break;
                default:
                    command = $"rc {Idle} {Idle} {Idle} {Idle}";
                    break;
            }
            SendCommandWithoutResponse(command);
        }

        /// <summary>
        /// Command drone to rotate.
        /// </summary>
        /// <param name="direction">The direction in which the drone should rotate.</param>
        /// <param name="speed">The speed in which the drone should rotate.</param>
        /// <returns></returns>
        public void Rotate(RotationDirection direction, int speed)
        {
            CommandConstraints.CheckRC(speed);
            string command;
            switch (direction)
            {
                case RotationDirection.Clockwise:
                    command = $"rc {Idle} {Idle} {Idle} {speed}";
                    break;
                case RotationDirection.CounterClockwise:
                    speed *= -1;
                    command = $"rc {Idle} {Idle} {Idle} {speed}";
                    break;
                default:
                    command = $"rc {Idle} {Idle} {Idle} {Idle}";
                    break;
            }
            SendCommandWithoutResponse(command);
        }

        /// <summary>
        /// Command to fly to a specific position with a specific speed.
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="z">Z Position</param>
        /// <param name="speed">Speed</param>
        /// <returns>Drone response</returns>
        public Task<bool> FlyTo(int x, int y, int z, int speed)
        {
            CommandConstraints.CheckDistance(x);
            CommandConstraints.CheckDistance(y);
            CommandConstraints.CheckDistance(z);
            CommandConstraints.CheckSpeed(speed);
            return SendCommandWithResponse($"go {x} {y} {z} {speed}");
        }

        // ToDo: Curve overload "mid"
        /// <summary>
        /// Command the drone to fly at a curve according to the two given coordinate at speed (cm/s).
        /// </summary>
        /// <param name="x1">x1 Coordinate</param>
        /// <param name="y1">y1 Coordinate</param>
        /// <param name="z1">z1 Coordinate</param>
        /// <param name="x2">x2 Coordniate</param>
        /// <param name="y2">y2 Coordinate</param>
        /// <param name="z2">z2 Coordinate</param>
        /// <param name="speed">The speed in cm/s</param>
        /// <returns></returns>
        public Task<bool> Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed)
        {
            CommandConstraints.CheckDistance(x1);
            CommandConstraints.CheckDistance(y1);
            CommandConstraints.CheckDistance(z1);
            CommandConstraints.CheckDistance(x2);
            CommandConstraints.CheckDistance(y2);
            CommandConstraints.CheckDistance(z2);
            CommandConstraints.CheckSpeed(speed);
            
            return SendCommandWithResponse($"curve {x1} {y1} {z1} {x2} {y2} {z2} {speed}");
        }

        #endregion

        #region TelloSetExtensions

        /// <summary>
        /// Sets the speed of the Tello drone.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns>Success Status</returns>
        public Task<bool> Speed( int speed)
        {
            CommandConstraints.CheckSpeed(speed);
            return SendCommandWithResponse($"speed {speed}");
        }


        /// <summary>
        /// Set the Wifi name and Passwort.
        /// </summary>
        /// <param name="ssid">The WiFi name.</param>
        /// <param name="pass">The WiFi password.</param>
        /// <returns></returns>
        public Task<bool> WiFi(string ssid, string pass)
        {
            return SendCommandWithResponse($"wifi {ssid} {pass}");
        }

        // ToDo: set mon
        // ToDo: set moff
        // ToDo: set mdirection x
        public Task<bool> Station(string ssid, string pass)
        {
            return SendCommandWithResponse($"ap {ssid} {pass}");
        }

        #endregion

        #region TelloReadExtensions

        public Task<string> GetSpeed()
        {
            return SendCommand("speed?");
        }

        public Task<string> GetBattery()
        {
            return SendCommand("battery?");
        }

        public Task<string> GetTime()
        {
            return SendCommand("time?");
        }

        public Task<string> GetWifi()
        {
            return SendCommand("wifi?");
        }

        public Task<string> GetSDK()
        {
            return SendCommand("sdk?");
        }

        public Task<string> GetSerialNumber()
        {
            return SendCommand("sn?");
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task<string> SendCommand(string command)
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
            
            string response = null;
            var ans = _udpClient.Receive(ref _endPoint);
            response = Encoding.ASCII.GetString(ans);
            
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<bool> SendCommandWithResponse(string command)
        {
            for (int attempt = 0; attempt < TelloSettings.RetryCount; attempt++)
            {
                var response = await SendCommand(command);
                
                if (response != null && String.CompareOrdinal(response.ToLower(), SuccessResponse) == 0)
                {
                    Logger.Trace($"Command '{command}' was successful");
                    return true;
                }
                // Logging
                Logger.Trace($"Command attempt {attempt} failed for command: '{command}'");
            }
            
            return false;
        }

        private void SendCommandWithoutResponse(string command)
        {
            byte[] datagram = Encoding.ASCII.GetBytes(command);
            _udpClient.SendAsync(datagram, datagram.Length); // wartet nur drauf, bis die Nachricht tatsächlich abgeschickt wurde.
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

        #endregion
    }
}
