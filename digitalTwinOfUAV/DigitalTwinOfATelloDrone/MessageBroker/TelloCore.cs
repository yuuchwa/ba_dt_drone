using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfATelloDrone.Logger;
using DigitalTwinOfATelloDrone.RyzeSDK.Attribute;
using DigitalTwinOfATelloDrone.RyzeSDK.Core;
using DigitalTwinOfATelloDrone.RyzeSDK.Models;
using DigitalTwinOfATelloDrone.RyzeSDK.Output;

// using Microsoft.Extensions.Logging;

namespace DigitalTwinOfATelloDrone.RyzeSDK;

/// <summary>
/// This class handels all communications with the tello drone.
/// </summary>
public class TelloCore : ICore
{
    private static TelloCore _telloCoreInstance;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger Logger;

    /// <summary>
    /// The Tello client
    /// </summary>
    private readonly ITelloClient _telloClient;
    
    /// <summary>
    /// The state server
    /// </summary>
    private readonly TelloStateServer _stateServer;

    private readonly ConsoleCockpit _consoleOutput;

    /// <summary>
    /// The Status where the core is connected to the drone or not.
    /// </summary>
    private bool _connectionStatus;
    
    /// <summary>
    /// Flag that indicates where the process should be stopped.
    /// </summary>
    private bool _stopThread;

    /// <summary>
    /// The FFmpeg
    /// </summary>
    //private readonly FFmpeg ffmpeg;

    private readonly Queue<DroneCommand> _commandQueue = null;

    //private Task _commandProcessor;
    private readonly Thread _commandHandlerThread;
    
    /// <summary>
    /// Token for terminating a thread.
    /// </summary>
    private CancellationTokenSource _commandProcessorCancellationToken;

    /// <summary>
    /// The state of the drone.
    /// </summary>
    private TelloStateParameter _telloStateParameter;

    public static TelloCore GetInstance()
    {
        return _telloCoreInstance ?? (_telloCoreInstance = new TelloCore());
    }

    /// <summary>
    /// Instantiates the Core
    /// </summary>
    // public Core(ILogger<Core> logger , TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
    //public TelloCore(TelloClient telloClient, TelloStateServer stateServer)
    private TelloCore()
    {
        _stopThread = false;

        Logger = new NLogLogger(GetType().Name);
        
        _commandQueue = new Queue<DroneCommand>();
        _commandHandlerThread = new Thread(ProcessCommandTask);
        _commandHandlerThread.Start();
        
        _telloClient = new TelloClient();
        _stateServer = new TelloStateServer();
        //_consoleOutput = new ConsoleDisplay(_stateServer);
        
        _stateServer.OnState += (s) => _telloStateParameter = s;
        _stateServer.OnException += (ex) => Logger.Log(new LogEntry(LoggingEventType.Error, "stateServer.OnException", ex));

        IntitializeConnectionToTello();
        
        _commandProcessorCancellationToken = new CancellationTokenSource();

        //this.ffmpeg = ffmpeg;
    }

    /// <summary>
    /// Initialize the communication the Tello drone.
    /// </summary>
    private async void IntitializeConnectionToTello()
    {
        _telloClient.Connect();
        _stateServer.Listen();
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
    
    public void QueryCommand(DroneCommand command)
    {
        _commandQueue.Enqueue(command);
    }

    private async void ProcessCommandTask()
    {
        while (!_stopThread)
        {
            DroneCommand command = null;
            if (_commandQueue.TryDequeue(out command))
            {
                TelloAction action = command._action;
                
                try
                {
                    bool response = true;

                    switch (action)
                    {
                        // Antwort wird ignoriert.
                        case TelloAction.MoveForward:
                            _telloClient.RemoteControl(0, command._value, 0, 0);
                            break;
                        case TelloAction.MoveBackward:
                            _telloClient.RemoteControl(0, -command._value, 0, 0);
                            break;
                        case TelloAction.MoveLeft:
                            _telloClient.RemoteControl(-command._value, 0, 0, 0);
                            break;
                        case TelloAction.MoveRight:
                            _telloClient.RemoteControl(command._value, 0, 0, 0);
                            break;
                        case TelloAction.Rise:
                            _telloClient.RemoteControl(0, 0, command._value, 0);
                            break;
                        case TelloAction.Sink:
                            _telloClient.RemoteControl(0, 0, -command._value, 0);
                            break;
                        case TelloAction.RotateLeft:
                            _telloClient.RemoteControl(0, 0, 0, -command._value);
                            break;
                        case TelloAction.RotateRight:
                            _telloClient.RemoteControl(0, 0, 0, command._value);
                            break;
                        case TelloAction.Stop:
                            _telloClient.RemoteControl(0, 0, 0, 0);
                            break;
                        case TelloAction.TakeOff:
                            response = await _telloClient.TakeOff();
                            break;
                        case TelloAction.Land:
                            response = await _telloClient.Land();
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
                            Console.WriteLine("aufgerufen");
                            response = await _telloClient.InitTello();
                            if (response)
                            {
                                Console.WriteLine("Tello successfully connected");
                            }
                            else
                            {
                                Console.WriteLine("Tello connection failed");
                            }
                            break;
                        default: 
                            _telloClient.Emergency(); 
                            break;
                    }
                }
                catch (Exception e)
                {
                    _connectionStatus = false;
                    continue;
                }
                finally
                {
                    _connectionStatus = true;
                    command = null;
                }

                // rufe extend action auf.
            }
            else
            {
                Thread.Sleep(200);
            }
        }
    }
}

