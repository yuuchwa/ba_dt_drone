using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DigitalTwinOfUAV.Model;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.TelloSDK.Core;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RyzeTelloSDK.Core;
using TelloApplication;
using Microsoft.Extensions.DependencyInjection;


namespace DigitalTwinOfUAV;

internal static class Program
{
    public static void Main(string[] args)
    {
        testTello();
        
        // The scenario consists of the model (represented by the model description)
        // and the simulation configuration (see config.json).
            
        // Create a new model description that holds all parts of the model (agents, entities, layers).
        var description = new ModelDescription();
            
        // Add layer types that are part of the scenario.
        description.AddLayer<VirtuelEnvironmentLayer>();
        //description.AddLayer<PoiLayer>();

        // Add agent types that are part of the scenario. For each, specify the layer type on which it lives.
        description.AddAgent<UAV, VirtuelEnvironmentLayer>();
            
        // Scenario definition: Specify the configuration file that contains the specification of the scenario.
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);
            
        // Create simulation task
        var task = SimulationStarter.Start(description, config);
            
        // Run simulation
        var loopResults = task.Run();
            
        // Feedback to user that simulation run was successful
        Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        
    }

    private async Task<Task> Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var console = serviceProvider.GetRequiredService<ConsoleWorker>();
        await console.MainLoop();
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging() // configure => configure.AddFile(@"logs\log-{Date}.txt")
            .AddSingleton<TelloConnectionSettings>()
            .AddSingleton<TelloClient>()
            .AddSingleton<TelloStateServer>()
            .AddSingleton<Core>()
            .AddSingleton<FFmpeg>()
            .AddSingleton<ConsoleWorker>();
    }
}

internal class Task
{
}