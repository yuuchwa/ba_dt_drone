using System;
using System.Collections.Generic;
using System.Threading;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.CommunicationInferfaces;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.RyzeSDK.Models;
using DtTelloDrone.RyzeSDK.Output;

// using Microsoft.Extensions.Logging;

namespace DtTelloDrone.MessageBroker;

/// <summary>
/// This class handels all communications with the tello drone.
/// </summary>
public class TelloMessageBroker : IDroneMessageBroker
{
    private static TelloMessageBroker _telloMessageBrokerInstance;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    private readonly ITelloClient _telloClient;
    private readonly TelloStateServer _stateServer;

    private readonly ConsoleCockpit _consoleOutput;
    
    private bool _connectionStatus;
    private bool _stopThread;

    private readonly Queue<TelloMessage> _commandQueue = null;
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
        
        _commandQueue = new Queue<TelloMessage>();
        _commandHandlerThread = new Thread(ProcessCommandTask);
        _commandHandlerThread.Start();
        
        _telloClient = new TelloClient();
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
        _telloClient.Connect();
        _stateServer.Listen();
    }

    private void PublishMessage(TelloMessage message)
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
        _telloClient.Disconnect();
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
    
    public void QueryCommand(TelloMessage command)
    {
        _commandQueue.Enqueue(command);
    }

    private async void ProcessCommandTask()
    {
        while (!_stopThread)
        {
            if (_commandQueue.TryDequeue(out TelloMessage message))
            {
                TelloTopic topic = message.GetTopic();
                switch (topic)
                {
                    case TelloTopic.DroneControl: 
                        PublishMessage(message);
                        ProcessDroneActionMessage(message.GetCommand()); 
                        break;
                    case TelloTopic.Operation: 
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

    private async void ProcessDroneActionMessage(Tuple<TelloAction, string> command)
    {
        if (command == null)
        {
            Logger.Error( $"Command valud was empty");
            return;
        }
        
        TelloAction action = command.Item1 ;
        if(!int.TryParse(command.Item2, out int value))
            Logger.Error($"Command value {value} was not a number");
            
        try
        {
            Logger.Info($"MessageBroker send action: {action}");
            switch (action)
            {
                // Antwort wird ignoriert.
                case TelloAction.MoveForward:
                    _telloClient.RemoteControl(0, value, 0, 0);
                    break;
                case TelloAction.MoveBackward:
                    _telloClient.RemoteControl(0, (-1) * value, 0, 0);
                    break;
                case TelloAction.MoveLeft:
                    _telloClient.RemoteControl((-1) * value, 0, 0, 0);
                    break;
                case TelloAction.MoveRight:
                    _telloClient.RemoteControl(value, 0, 0, 0);
                    break;
                case TelloAction.Rise:
                    _telloClient.RemoteControl(0, 0, value, 0);
                    break;
                case TelloAction.Sink:
                    _telloClient.RemoteControl(0, 0, (-1) * value, 0);
                    break;
                case TelloAction.RotateCounterClockwise:
                    _telloClient.RemoteControl(0, 0, 0, (-1) * value);
                    break;
                case TelloAction.RotateClockwise:
                    _telloClient.RemoteControl(0, 0, 0, value);
                    break;
                case TelloAction.Stop:
                    _telloClient.RemoteControl(0, 0, 0, 0);
                    break;
                case TelloAction.TakeOff:
                    await _telloClient.TakeOff();
                    break;
                case TelloAction.Land:
                    await _telloClient.Land();
                    break;
                case TelloAction.Emergency:
                    await _telloClient.Emergency();
                    break;
                case TelloAction.Speed:
                    await _telloClient.GetSpeed();
                    break;
                case TelloAction.Battery:
                    await _telloClient.GetBattery();
                    break;
                case TelloAction.Time:
                    await _telloClient.GetTime();
                    break;
                case TelloAction.Connect: 
                    if (await _telloClient.InitTello())
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
                    await _telloClient.Emergency(); 
                    break;
            }
        }
        catch (Exception e)
        { 
            _connectionStatus = false;
            Logger.Error($"Tello throwed error on action {action}");
        }

        // rufe extend action auf.
    }
}

