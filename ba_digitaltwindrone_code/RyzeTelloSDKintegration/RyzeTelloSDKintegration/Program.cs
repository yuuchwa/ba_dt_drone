using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RyzeTelloSDK.Enum;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDKintegration.Core;

namespace TelloTestApp
{
    class Program
    {
        /*
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                //.AddLogging(configure => configure.AddFile(@"logs\log-{Date}.txt"))
                .AddSingleton<TelloSettings>()
                .AddSingleton<TelloClient>()
                .AddSingleton<TelloStateServer>()
                .AddSingleton<TelloCore>()
                //.AddSingleton<FFmpeg>()
                //.AddSingleton<GamePadController>()
                .AddSingleton<ConsoleWorker>();
        }

        static async Task Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var console = serviceProvider.GetRequiredService<ConsoleWorker>();
            await console.MainLoop();
        }
        */
        
        public static async Task Main()
        {
            /*
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<TelloCore>();
            */
            //new TelloCore();
            long milliseconds1 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            TelloClient client = new TelloClient();
            
            Console.WriteLine("initialisiert");
            await client.Init();
            //Thread.Sleep(2000);
            
            Console.WriteLine("frage Battery");
            await client.GetBattery();

            //await client.TakeOff();
            
            //await client.FlyDirection(MoveDirection.Up, 20);
            //await client.FlyDirection(MoveDirection.Down, 20);

            //await client.Land();
            
            long milliseconds2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine(milliseconds2 - milliseconds1);
            
            while (true)
            {
                
            }
        }
    }
}