using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Agent;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;
using DtTelloDrone.RemoteControl.Control;
using DtTelloDrone.RyzeSDK;
using log4net;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace DtTelloDrone;

public static class Startup
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    private static readonly TelloMessageBroker TelloMessageBroker = TelloMessageBroker.GetInstance();

    private static KeyboardControl _keyboardControl;
    private static SimulationStarter _simulationStarter;

    public static void Run()
    {
        RunKeybordControl();
        RunSimulation();
        DisposeApplicationRessources();
    }

    private static void RunSimulation()
    {

        // Create a new model description that holds all parts of the model (agents, entities, layers).
        var description = new ModelDescription();
        description.AddLayer<LandScapeLayer>();
        description.AddAgent<TelloAgent, LandScapeLayer>();
                
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);
        
        _simulationStarter = SimulationStarter.Start(description, config);
        
        _keyboardControl.AddSimulation(_simulationStarter);

        var loopResults = _simulationStarter.Run();
                
        Console.WriteLine($"Simulation execution finished after {loopResults.Iterations} steps");
    }

    private static void RunKeybordControl()
    {
        _keyboardControl = new KeyboardControl();
        _keyboardControl.StartKeyboardControl();
    }

    private static void DisposeApplicationRessources()
    {
        _keyboardControl.Close();
        TelloMessageBroker.Close();
        RecordRepeatNavigationRecorder.Close();
        Logger.Info("Ressoures Disposed");
    }
}