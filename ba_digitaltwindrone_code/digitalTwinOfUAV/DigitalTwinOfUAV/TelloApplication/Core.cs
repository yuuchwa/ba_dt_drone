using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using System;
using System.Threading.Tasks;

namespace TelloApplication
{
    /// <summary>
    /// This class handels all communications with the tello drone.
    /// </summary>
    public class Core
    {
        
        #region Private Members

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;
        
        /// <summary>
        /// The Tello client
        /// </summary>
        private readonly TelloClient _client;
        
        /// <summary>
        /// The state server
        /// </summary>
        private readonly TelloStateServer _stateServer;
        
        /// <summary>
        /// The FFmpeg
        /// </summary>
        private readonly FFmpeg _ffmpeg;

        /// <summary>
        /// The state of the drone.
        /// </summary>
        private TelloState _telloState;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates the Core
        /// </summary>
        /// <param name="logger">The Logger</param>
        /// <param name="client">The Client</param>
        /// <param name="stateServer">The state server</param>
        /// <param name="ffmpeg">The ffmpeg</param>
        public Core(ILogger<Core> logger, TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
        {
            this._logger = logger;
            this._client = client;
            this._stateServer = stateServer;
            this._ffmpeg = ffmpeg;

            stateServer.OnState += (s) => _telloState = s;
            stateServer.OnException += (ex) => logger.LogError(ex, "stateServer.OnException");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the communication the Tello drone.
        /// </summary>
        public async Task InitializeCommunicationWithDrone()
        {
            _client.Connect();
            _stateServer.Listen();

            await TrySendCommand(_client.Init);
            await TrySendCommand(_client.StreamOn);
            _ffmpeg.Spawn();
        }

        /// <summary>
        /// Terminate all services.
        /// </summary>
        public void Close()
        {
            _client.Disconnect();
            _stateServer.Close();
            _ffmpeg.Close();
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

        #endregion

        #region Private Methods

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
                _logger.LogError(ex, $"Exception while invoking {function.Method.Name} function");
            }
            return default;
        }

        #endregion
    }
}
