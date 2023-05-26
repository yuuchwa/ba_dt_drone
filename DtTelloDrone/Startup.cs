using System;
using System.IO;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Agent;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;
using DtTelloDrone.RemoteControl.Control;
using DtTelloDrone.RemoteControl.Output;
using Mars.Components.Starter;
using Mars.Interfaces.Model;

namespace DtTelloDrone;

public static class Startup
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    private static readonly TelloMessageBroker TelloMessageBroker = TelloMessageBroker.GetInstance();

    private static KeyboardControl _keyboardControl;
    private static FlightDeck _flightDeck;
    private static SimulationStarter _simulationStarter;

    public static void Run()
    {
        RunKeybordControl();
        RunFlightDeck();
        RunSimulation();
        DisposeApplicationRessources();
    }

    private static void RunSimulation()
    {
        var description = new ModelDescription();
        description.AddLayer<LandScapeLayer>();
        description.AddAgent<TelloAgent, LandScapeLayer>();
                
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);
        
        _simulationStarter = SimulationStarter.Start(description, config);

        var loopResults = _simulationStarter.Run();
                
        Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
    }

    private static void RunKeybordControl()
    {
        _keyboardControl = new KeyboardControl();
        _keyboardControl.StartKeyboardControl();
    }

    private static void RunFlightDeck()
    {
        _flightDeck = new FlightDeck();
        _flightDeck.StartFlightDeck();
    }
    
    private static void DisposeApplicationRessources()
    {
        _keyboardControl.Close();
        TelloMessageBroker.Close();
        RecordRepeatNavigationRecorder.Close();
        Logger.Info("Ressoures Disposed");
    }
}