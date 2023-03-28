using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Components.Starter;
using Mars.Interfaces.Model;

namespace DigitalTwinOfUAV;

internal static class Program
{
    public static void Main(string[] args)
    {
        // The scenario consists of the model (represented by the model description)
        // and the simulation configuration (see config.json).
            
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
    
    private static async Task<Task> startTello()
    {
        //TelloClient client = new TelloClient();
        //TelloStateServer server = new TelloStateServer();
        TelloCore core = new TelloCore();
        Thread.Sleep(2000);
        /*
        DroneCommand command = new DroneCommand(TelloAction.Time, 0);
        Console.WriteLine("send");
        while (true)
        {
            core.QueryCommand(command);
            Thread.Sleep(500);
        }
        */
        char key = ' ';
        while (key != 'y')
        {
            var keyInfo = Console.ReadKey();
            key = keyInfo.KeyChar;
        }

        core.Close();
        return null;
    }
}