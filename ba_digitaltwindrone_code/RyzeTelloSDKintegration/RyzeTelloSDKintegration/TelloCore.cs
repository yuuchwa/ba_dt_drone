using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using RyzeTelloSDKintegration.Core;

// using Microsoft.Extensions.Logging;

namespace TelloTestApp
{
    /// <summary>
    /// This class handels all communications with the tello drone.
    /// </summary>
    public class TelloCore
    {
    	/// <summary>
        /// The logger.
        /// </summary>
        // private readonly ILogger logger;
        
        /// <summary>
        /// The Tello client
        /// </summary>
        private readonly TelloClient _telloClient;
        
        /// <summary>
        /// The state server
        /// </summary>
        private readonly TelloStateServer _stateServer;
        
        /// <summary>
        /// The FFmpeg
        /// </summary>
        //private readonly FFmpeg ffmpeg;

	/// <summary>
        /// The state of the drone.
        /// </summary>
        private TelloState _telloState;

        /// <summary>
        /// Instantiates the Core
        /// </summary>
        // public Core(ILogger<Core> logger , TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
        //public TelloCore(TelloClient telloClient, TelloStateServer stateServer)
        public TelloCore()
        {
            // this.logger = logger;
            this._telloClient = new TelloClient();
            this._stateServer = new TelloStateServer();
            //this.ffmpeg = ffmpeg;

            _stateServer.OnState += (s) => _telloState = s;
            // Init();
            //stateServer.OnException += (ex) => logger.LogError(ex, "stateServer.OnException");
        }

        /// <summary>
        /// Initialize the communication the Tello drone.
        /// </summary>
        public void IntitializeConnectionToTello()
        {
            _telloClient.Connect();
            _stateServer.Listen();
            TrySendCommand(_telloClient.Init); 
            TrySendCommand(_telloClient.StreamOn);
            //ffmpeg.Spawn();
        }

        /// <summary>
        /// Terminate all services.
        /// </summary>
        public void Close()
        {
            _telloClient.Disconnect();
            _stateServer.Close();
            //ffmpeg.Close();
        }

        /// <summary>
        /// Request the state of the drone.
        /// </summary>
        /// <returns>Return the current state.</returns>
        public TelloState GetState()
        {
            if (_telloState == null) return new TelloState();
            return _telloState;
        }

        /// <summary>
        /// Tries to send a command.
        /// </summary>
        /// <param name="function">The command for the drone.</param>
        /// <typeparam name="T">Type of command</typeparam>
        /// <returns>Operation response.</returns>
        private async Task<T> TrySendCommand<T>(Func<Task<T>> function)
        {
            try
            {
                return await function.Invoke();
                // logger.LogError(ex, $"Connection to Tello established");
            }
            catch (Exception ex)
            {
                // logger.LogError(ex, $"Exception while invoking {function.Method.Name} function");
            }
            return default;
        }
    }
}
