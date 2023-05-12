using System;
using System.Collections.Generic;
using System.Threading;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Output;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.TelloSdk.CommunicationInferfaces;
using DtTelloDrone.TelloSdk.DataModels;

// using Microsoft.Extensions.Logging;

namespace DtTelloDrone.MessageBroker;

/// <summary>
/// The TelloMessageBroker class handles all communications with the Tello drone.
/// </summary>
public class TelloMessageBroker : IDroneMessageBroker
{
    private static TelloMessageBroker _telloMessageBrokerInstance;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    private readonly IDroneClient _droneClient;
    private readonly IDroneServer _stateServer;

    private readonly ConsoleCockpit _consoleOutput;
    
    private bool _droneConnected;
    private bool _commandModeActive;
    private bool _stopThread;

    private readonly Queue<DroneMessage> _commandQueue = null;
    private readonly Thread _commandHandlerThread;

    private readonly List<IMessageBrokerSubscriber> _subscribers = new();

    /// <summary>
    /// Token for terminating a thread.
    /// </summary>
    private CancellationTokenSource _commandProcessorCancellationToken;

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
        _stateServer = new DroneStateServer();
        //_consoleOutput = new ConsoleCockpit(_stateServer);

        IntitializeConnectionToTello();
        
        _commandProcessorCancellationToken = new CancellationTokenSource();
    }
    
    /// <summary>
    /// The method add a new subscriber to the list of subscribers.
    /// </summary>
    /// <param name="subscriber">The subscriber ti be added</param>
    public void Subscribe(IMessageBrokerSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    /// <summary>
    /// Check whether the drone is active.
    /// </summary>
    /// <returns>true, if active</returns>
    public bool DroneConnected()
    {
        return _droneConnected;
    }

    /// <summary>
    /// check whether the drone is set to receive commands.
    /// </summary>
    /// <returns>true, if set to receive commands.</returns>
    public bool DroneIsInCommandMode()
    {
        return _commandModeActive;
    }

    /// <summary>
    /// Connect to the ports which are used to send commands and receive drone information.
    /// </summary>
    private async void IntitializeConnectionToTello()
    {
        _droneClient.Connect();
        _stateServer.Listen();
    }

    /// <summary>
    /// Publish a message to the subscribers.
    /// </summary>
    /// <param name="message"></param>
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
        _droneClient.Dispose();
        _stateServer.Close();
    }

    /// <summary>
    /// Request the state of the drone.
    /// </summary>
    /// <returns>Return the current state.</returns>
    public TelloStateParameter GetStateParameter()
    {
        return _stateServer.GetStateParameter();
    }
    
    /// <summary>
    /// Query a message to the command queue.
    /// </summary>
    /// <param name="command">The command.</param>
    public void QueryMessage(DroneMessage command)
    {
        _commandQueue.Enqueue(command);
    }

    /// <summary>
    /// A task which process the messages in the command queue.
    /// </summary>
    private void ProcessCommandTask()
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

    /// <summary>
    /// Process the action message and send the message to the drone.
    /// (Not called if the drone is not connected to the dronesystem)
    /// </summary>
    /// <param name="command">Tuple containing the drone action and value.</param>
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
            switch (action)
            {
                // Antwort wird ignoriert.
                case DroneAction.MoveForward:
                    _droneClient.Fly(MoveDirection.Forward, value);
                    break;
                case DroneAction.MoveBackward:
                    _droneClient.Fly(MoveDirection.Back, value);
                    break;
                case DroneAction.MoveLeft:
                    _droneClient.Fly(MoveDirection.Left, value);
                    break;
                case DroneAction.MoveRight:
                    _droneClient.Fly(MoveDirection.Right, value);
                    break;
                case DroneAction.Rise:
                    _droneClient.Fly(MoveDirection.Rise, value);
                    break;
                case DroneAction.Sink:
                    _droneClient.Fly(MoveDirection.Sink, value);
                    break;
                case DroneAction.RotateCounterClockwise:
                    _droneClient.Rotate(RotationDirection.CounterClockwise, value);
                    break;
                case DroneAction.RotateClockwise:
                    _droneClient.Rotate(RotationDirection.Clockwise, value);
                    break;
                case DroneAction.Stop:
                    _droneClient.Fly(MoveDirection.Stop, 0);
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
                    await _droneClient.EnableCommandMode();
                    _droneConnected = true;
                    break;
                case DroneAction.Disconnect:
                    _droneClient.DisableCommandMode();
                    _droneConnected = false;
                    break;
                default: 
                    await _droneClient.Emergency(); 
                    break;
            }
        }
        catch (Exception e)
        { 
            _droneConnected = false;
            Logger.Info($"Socket Timeout. System is not yet connected to the drone");
        }
    }
}

