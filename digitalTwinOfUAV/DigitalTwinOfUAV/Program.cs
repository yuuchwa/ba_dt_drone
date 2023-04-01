using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfUAV.Model.Agent;
using DigitalTwinOfUAV.Model.Layer;
using DigitalTwinOfUAV.RyzeSDK;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using MQTTnet.Client.ExtendedAuthenticationExchange;
using NetTopologySuite.Operation.Valid;
using RyzeTelloSDKintegration.Core;

namespace DigitalTwinOfUAV;

internal static class Program
{
    public static void Main(string[] args)
    {
        //runSimulation();
        runPlayground();
    }

    public static void runSimulation()
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
        TelloClient client = new TelloClient();
        client.Connect();
        Thread.Sleep(2000);
        client.SendCommandWithResponse("takeoff");
        Thread.Sleep(2000);

        client.SendCommandWithResponse("rc 0 10 0 0");
        Thread.Sleep(2000);

        client.SendCommandWithResponse("rc 0 -10 0 0");
        Thread.Sleep(2000);

        client.Land();
    }
}