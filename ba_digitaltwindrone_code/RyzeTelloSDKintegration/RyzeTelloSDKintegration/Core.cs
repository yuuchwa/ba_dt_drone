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
        private readonly TelloClient client;
        
        /// <summary>
        /// The state server
        /// </summary>
        private readonly TelloStateServer stateServer;
        
        /// <summary>
        /// The FFmpeg
        /// </summary>
        //private readonly FFmpeg ffmpeg;

	/// <summary>
        /// The state of the drone.
        /// </summary>
        private TelloState telloState;

        /// <summary>
        /// Instantiates the Core
        /// </summary>
        /// <param name="logger">The Logger</param>
        /// <param name="client">The Client</param>
        /// <param name="stateServer">The state server</param>
        /// <param name="ffmpeg">The ffmpeg</param>
        // public Core(ILogger<Core> logger , TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
        public Core(TelloClient client, TelloStateServer stateServer)
        {
            // this.logger = logger;
            this.client = client;
            this.stateServer = stateServer;
            //this.ffmpeg = ffmpeg;

            stateServer.OnState += (s) => telloState = s;
            //stateServer.OnException += (ex) => logger.LogError(ex, "stateServer.OnException");
        }

        /// <summary>
        /// Initialize the communication the Tello drone.
        /// </summary>
        public async Task Init()
        {
            client.Connect();
            Console.WriteLine("connecting");
            stateServer.Listen();
            Console.WriteLine("Connection successful");

            await TrySendCommand(client.Init);
            await TrySendCommand(client.StreamOn);
            //ffmpeg.Spawn();
        }

        /// <summary>
        /// Terminate all services.
        /// </summary>
        public void Close()
        {
            client.Disconnect();
            stateServer.Close();
            //ffmpeg.Close();
        }

        /// <summary>
        /// Request the state of the drone.
        /// </summary>
        /// <returns>Return the current state.</returns>
        public TelloState GetState()
        {
            if (telloState == null) return new TelloState();
            return telloState;
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
