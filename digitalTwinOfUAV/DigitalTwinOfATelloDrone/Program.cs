using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DigitalTwinOfATelloDrone.Model.Agent;
using DigitalTwinOfATelloDrone.Model.Layer;
using DigitalTwinOfATelloDrone.RemoteControl.Control;
using log4net;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;


namespace DigitalTwinOfATelloDrone;

internal static class Program
{

    public static void Main(string[] args)
    {
        ConsoleWorker consoleWorker = new ConsoleWorker();
        consoleWorker.Listen();

        runSimulation();
        //runPlayground();
    }

    private static void runSimulation()
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

    public static async void runPlayground()
    {


    }
}
