using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;

using RyzeTelloSDKintegration.Core;
using System.Globalization;
using RyzeTelloSDKintegration;
using System.Diagnostics.Tracing;
using RyzeTelloSDK.Extensions;

namespace TelloTestApp
{
    class Program
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                //.AddLogging(configure => configure.AddFile(@"logs\log-{Date}.txt"))
                .AddSingleton<TelloClient>()
                .AddSingleton<TelloStateServer>()
                .AddSingleton<TelloCore>();
                //.AddSingleton<FFmpeg>()
                //.AddSingleton<GamePadController>()
                // .AddSingleton<ConsoleWorker>();
        }

        //Dependency Injection
        // ServiceCollection: https://www.stevejgordon.co.uk/aspnet-core-dependency-injection-what-is-the-iservicecollection
        // ServiceProvicer: https://www.stevejgordon.co.uk/aspnet-core-dependency-injection-what-is-the-iserviceprovider-and-how-is-it-built
        static async Task Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            //var serviceCollection = new ServiceCollection();
            //ConfigureServices(serviceCollection);
            //var serviceProvider = serviceCollection.BuildServiceProvider();

            //var core = serviceProvider.GetRequiredService(TelloCore);
            //long milliseconds1 = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var client = new TelloClient();
            var server = new TelloStateServer();
            var video = new FFmpeg();

            TelloCore core = new TelloCore(client, server);
            video.Spawn();
            // await client.StreamOn();
            
            bool end = false;

            //long milliseconds2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //Console.WriteLine(milliseconds2 - milliseconds1);
            char key = ' ';
            while (key != 'y')
            {
                var keyInfo = Console.ReadKey();
                key = keyInfo.KeyChar;
            }

            core.Close();
        }
    }
}