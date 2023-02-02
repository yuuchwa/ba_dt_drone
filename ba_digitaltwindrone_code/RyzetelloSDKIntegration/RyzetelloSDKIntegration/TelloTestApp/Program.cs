using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using System.Globalization;
using System.Threading.Tasks;

namespace TelloTestApp
{
    class Program
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                //.AddLogging(configure => configure.AddFile(@"logs\log-{Date}.txt"))
                .AddSingleton<TelloSettings>()
                .AddSingleton<TelloClient>()
                .AddSingleton<TelloStateServer>()
                .AddSingleton<Core>()
                .AddSingleton<FFmpeg>()
                .AddSingleton<GamePadController>()
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
    }
}