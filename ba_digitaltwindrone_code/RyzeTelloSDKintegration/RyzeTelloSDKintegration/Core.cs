using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using System;
using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;

namespace TelloTestApp
{
    public class Core
    {
        // private readonly ILogger logger;
        private readonly TelloClient client;
        private readonly TelloStateServer stateServer;
        //private readonly FFmpeg ffmpeg;

        private TelloState telloState;

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

        public void Close()
        {
            client.Disconnect();
            stateServer.Close();
            //ffmpeg.Close();
        }

        public TelloState GetState()
        {
            if (telloState == null) return new TelloState();
            return telloState;
        }

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
