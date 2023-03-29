using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Extensions;
using RyzeTelloSDK.Models;
using RyzeTelloSDKintegration.Core;
using RyzeTelloSDKintegration.FlightManagementSystem;
using System.Threading;
using System.Threading.Tasks;
using RyzeTelloSDK.Enum;

// using Microsoft.Extensions.Logging;

namespace DigitalTwinOfUAV.RyzeSDK;

/// <summary>
/// This class handels all communications with the tello drone.
/// </summary>
public class TelloCore : ICore
{
    /// <summary>
    /// The logger.
    /// </summary>
    // private readonly ILogger logger;
    
    /// <summary>
    /// The Tello client
    /// </summary>
    private readonly TelloClient _telloClient;
    
    /// <summary>
    /// The state server
    /// </summary>
    private readonly TelloStateServer _stateServer;

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

    /// <summary>
    /// Instantiates the Core
    /// </summary>
    // public Core(ILogger<Core> logger , TelloClient client, TelloStateServer stateServer, FFmpeg ffmpeg)
    //public TelloCore(TelloClient telloClient, TelloStateServer stateServer)
    public TelloCore()
    {
        _stopThread = false;

        _commandQueue = new Queue<DroneCommand>();
        _commandHandlerThread = new Thread(ProcessCommandTask);
        _commandHandlerThread.Start();
        
        _telloClient = new TelloClient();
        _stateServer = new TelloStateServer();
        //_consoleOutput = new ConsoleDisplay(_stateServer);
        
        _stateServer.OnState += (s) => _telloStateParameter = s;
        
        IntitializeConnectionToTello();
        
        _commandProcessorCancellationToken = new CancellationTokenSource();
        // _commandProcessor = Task.Run(ProcessCommandTask, _commandProcessorCancellationToken.Token);

        //this.ffmpeg = ffmpeg;
        //stateServer.OnException += (ex) => logger.LogError(ex, "stateServer.OnException");
    }

    /// <summary>
    /// Initialize the communication the Tello drone.
    /// </summary>
    private async void IntitializeConnectionToTello()
    {
        _telloClient.Connect();
        _stateServer.Listen();
        await _telloClient.InitTello();
        Console.WriteLine("Inizialization successful");
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
        return _telloStateParameter ?? new TelloStateParameter();
    }
    
    public void QueryCommand(DroneCommand command)
    {
        _commandQueue.Enqueue(command);
    }

    private async void ProcessCommandTask()
    {
        DroneCommand command = null;
        while (!_stopThread)
        {
            if (_commandQueue.TryDequeue(out command))
            {
                TelloAction action = command._action;
                string res;
                try
                {
                    switch (action)
                    {
                        // Antwort wird ignoriert.
                        case TelloAction.MoveForward:
                            await _telloClient.FlyDirection(MoveDirection.Forward, 30);
                            break;
                        case TelloAction.MoveBackward:
                            await _telloClient.FlyDirection(MoveDirection.Back, 30);
                            break;
                        case TelloAction.MoveLeft:
                            await _telloClient.FlyDirection(MoveDirection.Left, 30);
                            break;
                        case TelloAction.MoveRight:
                            await _telloClient.FlyDirection(MoveDirection.Right, 30);
                            break;
                        case TelloAction.Rise:
                            await _telloClient.FlyDirection(MoveDirection.Up, 30);
                            break;
                        case TelloAction.Sink:
                            await _telloClient.FlyDirection(MoveDirection.Down, 30);
                            break;
                        case TelloAction.RotateLeft:
                            await _telloClient.RotateDirection(RotationDirection.Clockwise, 20);
                            break;
                        case TelloAction.RotateRight:
                            await _telloClient.RotateDirection(RotationDirection.CounterClockwise, 20);
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
                            res = await _telloClient.GetBattery();
                            Console.WriteLine(res);
                            break;
                        case TelloAction.Time:
                            await _telloClient.GetTime();
                            break;
                        
                        default: break;
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
    
    /// <summary>
    /// Tries to send a command.
    /// </summary>
    /// <param name="function">The command for the drone.</param>
    /// <typeparam name="T">Type of command</typeparam>
    /// <returns>Operation response.</returns>
    private async Task<T> TrySendCommand<T>(Func<Task<T>> function)
    {
        try
        {
            return await function.Invoke();
            // logger.LogError(ex, $"Connection to Tello established");
        }
        catch (Exception ex)
        {
            // logger.LogError(ex, $"Exception while invoking {function.Method.Name} function");
        }
        return default;
    }
}

