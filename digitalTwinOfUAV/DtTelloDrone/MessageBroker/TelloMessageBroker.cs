using System;
using System.Collections.Generic;
using System.Threading;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.CommunicationInferfaces;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;
using DtTelloDrone.RyzeSDK.Output;
using DtTelloDrone.TelloSdk.CommunicationInferfaces;

// using Microsoft.Extensions.Logging;

namespace DtTelloDrone.MessageBroker;

/// <summary>
/// This class handels all communications with the tello drone.
/// </summary>
public class TelloMessageBroker : IDroneMessageBroker
{
    private static TelloMessageBroker _telloMessageBrokerInstance;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    private readonly IDroneClient _droneClient;
    private readonly TelloStateServer _stateServer;

    private readonly ConsoleCockpit _consoleOutput;
    
    private bool _connectionStatus;
    private bool _stopThread;

    private readonly Queue<DroneMessage> _commandQueue = null;
    private readonly Thread _commandHandlerThread;

    private readonly List<IMessageBrokerSubscriber> _subscribers = new();

    /// <summary>
    /// Token for terminating a thread.
    /// </summary>
    private CancellationTokenSource _commandProcessorCancellationToken;

    /// <summary>
    /// The state of the drone.
    /// </summary>
    private TelloStateParameter _telloStateParameter;

    public static TelloMessageBroker GetInstance()
    {
        return _telloMessageBrokerInstance ??= new TelloMessageBroker();
    }

    /// <summary>
    /// Instantiates the Core
    /// </summary>
    private TelloMessageBroker()
    {
        _stopThread = false;
        
        _commandQueue = new Queue<DroneMessage>();
        _commandHandlerThread = new Thread(ProcessCommandTask);
        _commandHandlerThread.Start();
        
        _droneClient = new DroneClient();
        _stateServer = new TelloStateServer();
        //_consoleOutput = new ConsoleCockpit(_stateServer);
        
        _stateServer.OnState += (s) => _telloStateParameter = s;
        _stateServer.OnException += (ex) => Logger.Error("stateServer.OnException");

        IntitializeConnectionToTello();
        
        _commandProcessorCancellationToken = new CancellationTokenSource();

        //this.ffmpeg = ffmpeg;
    }
    
    public void Subscribe(IMessageBrokerSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }
    
    /// <summary>
    /// Initialize the communication the Tello drone.
    /// </summary>
    private async void IntitializeConnectionToTello()
    {
        _droneClient.Connect();
        _stateServer.Listen();
    }

    private void PublishMessage(DroneMessage message)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.PublishMessage(message);
        }
    }

    /// <summary>
    /// Close and dispose.
    /// </summary>
    public void Close()
    {
        // gegebenenfalls warten, bis der letzte Command ausgeführt wurde.
        _stopThread = true;
        _commandHandlerThread.Join();

        _commandQueue.Clear();
        _commandProcessorCancellationToken.Cancel();
        _droneClient.Disconnect();
        _stateServer.Close();

        //ffmpeg.Close();
    }

    /// <summary>
    /// Request the state of the drone.
    /// </summary>
    /// <returns>Return the current state.</returns>
    public TelloStateParameter GetStateParameter()
    {
        return _telloStateParameter;
    }
    
    public void QueryMessage(DroneMessage command)
    {
        _commandQueue.Enqueue(command);
    }

    private async void ProcessCommandTask()
    {
        while (!_stopThread)
        {
            if (_commandQueue.TryDequeue(out DroneMessage message))
            {
                MessageTopic topic = message.GetTopic();
                switch (topic)
                {
                    case MessageTopic.DroneCommand: 
                        PublishMessage(message);
                        ProcessDroneActionMessage(message.GetCommand()); 
                        break;
                    case MessageTopic.Operation: 
                        PublishMessage(message);
                        break;
                    default: break;
                }
            }
            else
            {
                Thread.Sleep(200);
            }
        }
    }

    private async void ProcessDroneActionMessage(Tuple<DroneAction, string> command)
    {
        if (command == null)
        {
            Logger.Error( $"Command valud was empty");
            return;
        }
        
        DroneAction action = command.Item1 ;
        if(!int.TryParse(command.Item2, out int value))
            Logger.Error($"Command value {value} was not a number");
            
        try
        {
            Logger.Info($"MessageBroker send action: {action}");
            switch (action)
            {
                // Antwort wird ignoriert.
                case DroneAction.MoveForward:
                    _droneClient.RemoteControl(0, value, 0, 0);
                    break;
                case DroneAction.MoveBackward:
                    _droneClient.RemoteControl(0, (-1) * value, 0, 0);
                    break;
                case DroneAction.MoveLeft:
                    _droneClient.RemoteControl((-1) * value, 0, 0, 0);
                    break;
                case DroneAction.MoveRight:
                    _droneClient.RemoteControl(value, 0, 0, 0);
                    break;
                case DroneAction.Rise:
                    _droneClient.RemoteControl(0, 0, value, 0);
                    break;
                case DroneAction.Sink:
                    _droneClient.RemoteControl(0, 0, (-1) * value, 0);
                    break;
                case DroneAction.RotateCounterClockwise:
                    _droneClient.RemoteControl(0, 0, 0, (-1) * value);
                    break;
                case DroneAction.RotateClockwise:
                    _droneClient.RemoteControl(0, 0, 0, value);
                    break;
                case DroneAction.Stop:
                    _droneClient.RemoteControl(0, 0, 0, 0);
                    break;
                case DroneAction.TakeOff:
                    await _droneClient.TakeOff();
                    break;
                case DroneAction.Land:
                    await _droneClient.Land();
                    break;
                case DroneAction.EmergencyLanding:
                    await _droneClient.Emergency();
                    break;
                case DroneAction.Speed:
                    await _droneClient.GetSpeed();
                    break;
                case DroneAction.Battery:
                    await _droneClient.GetBattery();
                    break;
                case DroneAction.Time:
                    await _droneClient.GetTime();
                    break;
                case DroneAction.Connect: 
                    if (await _droneClient.InitDrone())
                    {
                        _connectionStatus = true;
                        Logger.Info("Message Broker successfully connected to Tello");
                    }
                    else
                    {
                        Logger.Info("Connectio to Tello failed");
                    }
                    break;
                default: 
                    await _droneClient.Emergency(); 
                    break;
            }
        }
        catch (Exception e)
        { 
            _connectionStatus = false;
            Logger.Error($"Tello throwed error on action {action}");
        }
    }
}

