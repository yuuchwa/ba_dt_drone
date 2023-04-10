using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DtTelloDrone.Model.Agent;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.RemoteControl.Control;
using log4net;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;


namespace DtTelloDrone;

internal static class Program
{
    public static void Main(string[] args)
    {
        ConsoleWorker consoleWorker = new ConsoleWorker();
        consoleWorker.Listen();

        RunSimulation();
        //runPlayground();
    }

    private static void RunSimulation()
    {

        // Create a new model description that holds all parts of the model (agents, entities, layers).
        var description = new ModelDescription();
        description.AddLayer<LandScapeLayer>();
        description.AddAgent<TelloAgent, LandScapeLayer>();
                
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);
                
        var task = SimulationStarter.Start(description, config);
                
        var loopResults = task.Run();
                
        Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
    }

    public static async void RunPlayground()
    {


    }
}
