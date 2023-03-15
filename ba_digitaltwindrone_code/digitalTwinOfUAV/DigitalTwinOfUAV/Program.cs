using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfUAV.Model;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Enum;
using RyzeTelloSDKintegration.Core;

namespace DigitalTwinOfUAV;

internal static class Program
{
    public static void Main(string[] args)
    {
        startTello();
        /*
        // The scenario consists of the model (represented by the model description)
        // and the simulation configuration (see config.json).
            
        // Create a new model description that holds all parts of the model (agents, entities, layers).
        var description = new ModelDescription();
            
        // Add layer types that are part of the scenario.
        description.AddLayer<BaseLayer>();
        //description.AddLayer<PoiLayer>();

        // Add agent types that are part of the scenario. For each, specify the layer type on which it lives.
        description.AddAgent<TelloAgent, BaseLayer>();
            
        // Scenario definition: Specify the configuration file that contains the specification of the scenario.
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);
            
        // Create simulation task
        var task = SimulationStarter.Start(description, config);
            
        // Run simulation
        var loopResults = task.Run();
            
        // Feedback to user that simulation run was successful
        Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
        */
    }
    
    private static async Task<Task> startTello()
    {
        //TelloClient client = new TelloClient();
        //TelloStateServer server = new TelloStateServer();
        TelloCore core = new TelloCore();
        Thread.Sleep(2000);
        DroneCommand command = new DroneCommand(TelloAction.Time, 0);
        Console.WriteLine("send");
        while (true)
        {
            core.QueryCommand(command);
            Thread.Sleep(500);
        }
    }
}