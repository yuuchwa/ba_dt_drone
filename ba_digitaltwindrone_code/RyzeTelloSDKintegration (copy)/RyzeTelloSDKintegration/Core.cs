using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using System;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace TelloTestApp
{
    /// <summary>
    /// This class handels all communications with the tello drone.
    /// </summary>
    public class Core
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
        /// <param name="logger">The Logger</param>
        /// <param name="telloClient">The Client</param>
        /// <param name="stateServer">The state server</param>
        /// <param name="ffmpeg">The ffmpeg</param>
        // public Core(ILogger<Core> logger , TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
        public Core(TelloClient telloClient, TelloStateServer stateServer)
        {
            // this.logger = logger;
            this._telloClient = telloClient;
            this._stateServer = stateServer;
            //this.ffmpeg = ffmpeg;

            stateServer.OnState += ReceiveState; // (s) => _telloState = s;
            //stateServer.OnException += (ex) => logger.LogError(ex, "stateServer.OnException");
        }

        public void ReceiveState(TelloState state)
        {
            Console.WriteLine("write new State");
            _telloState = state;
        }

        /// <summary>
        /// Initialize the communication the Tello drone.
        /// </summary>
        public async Task Init()
        {
            _telloClient.Connect();
            Console.WriteLine("connecting");
            _stateServer.Listen(); // Startet den Thread zum Abhören
            Console.WriteLine("Connection successful");

            await TrySendCommand(_telloClient.Init);
            await TrySendCommand(_telloClient.StreamOn);
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
            }
            catch (Exception ex)
            {
                // logger.LogError(ex, $"Exception while invoking {function.Method.Name} function");
            }
            return default;
        }
    }
}
