using System;
using System.IO;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TelloTestApp;

namespace DigitalTwinOfUAV;

internal static class Program
{
    public static void Main(string[] args)
    {
        ConsoleWorker consoleWorker = new ConsoleWorker();
        consoleWorker.Listen();

        runSimulation();
        //runPlayground();

        while (true)
        {
            
        }        
    }

    private static void runSimulation()
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

    public static async void runPlayground()
    {
        
    }
}