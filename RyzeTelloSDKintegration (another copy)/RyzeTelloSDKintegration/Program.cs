using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;

using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using RyzeTelloSDKintegration.Core;
using System.Drawing;
using RyzeTelloSDKintegration;


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
        static TelloState telloState = null;

        public static async Task Main()
        {

            //new TelloCore();
            //long milliseconds1 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            TelloCore core = new TelloCore();
            bool end = false;
            /*
            TelloClient client = new TelloClient();
            await client.Init();
            string battery = await client.GetBattery();
            Console.WriteLine(battery);
            
            TelloStateServer server = new TelloStateServer();
            server.OnState += ReceiveState;
            server.Listen();
            */
            
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
        
        public static TelloState GetState()
        {
            if (telloState == null) return new TelloState();
            return telloState;
        }
        
        private static void RenderConsole(TelloState state, bool firstTime = true)
        {
            if (firstTime)
            {
                // Console.SetWindowSize(90, 9); funktioniert nicht
                Console.SetCursorPosition(15, 0);
                Console.Write("      X      Y      Z                    Low. High. °C     Pitch  Roll  Yaw");
                Console.SetCursorPosition(0, 1);
                Console.Write("Acceleration:                              Temperature:");
                Console.SetCursorPosition(0, 2);
                Console.Write("Velocity:                                   TOF  Height  Battery  Barometer");
                Console.SetCursorPosition(0, 3);
                Console.Write("Time:");
                Console.SetCursorPosition(0, 4);
                Console.Write("Gamepad:");
                Console.SetCursorPosition(0, 8);
                Console.Write(">");
            }
            Console.SetCursorPosition(16, 1);
            Console.Write($"{state.AccelerationX,6:0} {state.AccelerationY,6:0} {state.AccelerationZ,6:0}");
            Console.SetCursorPosition(16, 2);
            Console.Write($"{state.VelocityX,6:0} {state.VelocityY,6:0} {state.VelocityZ,6:0}");
            Console.SetCursorPosition(57, 1);
            Console.Write($"{state.TempLowest,3}", GetTempColor(state.TempLowest));
            Console.Write($"   {state.TempHighest,3}", GetTempColor(state.TempHighest));
            Console.SetCursorPosition(74, 1);
            Console.Write($"{state.Pitch,5}  {state.Roll,4}  {state.Yaw,3}");
            Console.SetCursorPosition(44, 3);
            Console.Write($"{state.TOF,3}  {state.Height,4}cm  {state.Battery,6}%  {state.Barometer,9:0.000}");
            Console.SetCursorPosition(6, 3);
            Console.Write($"{state.Time}s");
            Console.SetCursorPosition(9, 4);

            Console.SetCursorPosition(2, 8);
        }

        private static Color GetTempColor(int temp)
        {
            if (temp > 80) return Color.Red;
            if (temp > 60) return Color.Orange;
            if (temp > 50) return Color.Yellow;
            return Color.Green;
        }
        
        public static void ReceiveState(TelloState state)
        {
            Console.WriteLine("write new State");
            telloState = state;
        }
    }
}