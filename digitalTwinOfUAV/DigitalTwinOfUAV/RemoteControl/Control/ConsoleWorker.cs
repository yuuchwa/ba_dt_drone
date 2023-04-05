using Microsoft.Extensions.Logging;
using RyzeTelloSDK.Core;
using RyzeTelloSDK.Enum;
using RyzeTelloSDK.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using DigitalTwinOfUAV.RyzeSDK;

namespace TelloTestApp
{
    public class ConsoleWorker
    {
        // private readonly ILogger logger;
        
        /// <summary>
        /// Core Service
        /// </summary>
        private readonly ICore _telloCore = TelloCore.GetTelloCoreInstance();

        private Task _mainloop;

        private int _speed = 30;
        private int _rotationSpeed = 60;

        /// <summary>
        /// Token for canceling a thread.
        /// </summary>
        private CancellationTokenSource _cancellationToken;
        
        public ConsoleWorker()
        {

        }

        public void Close()
        {
            _cancellationToken.Cancel();
            Console.WriteLine("ConsoleWorker terminated");
        }

        public void Listen()
        {
            _cancellationToken = new CancellationTokenSource();
            _mainloop = Task.Run(StartConsoleWorker, _cancellationToken.Token);
        }

        private async void StartConsoleWorker()
        {
            DroneCommand command;
            while (true)
            {
                TelloAction selectedAction = ReadKeyboard();

                if (selectedAction == TelloAction.Unknown)
                {
                    command = new DroneCommand(selectedAction, _speed); 
                
                    // Check Command validity
                    /*
                    if (selectedAction != TelloAction.Unknown)
                    {
                        DroneCommand command = new DroneCommand(selectedAction, _speed);
            
                        if (IsMovementAction(selectedAction))
                        {
                            if (!CheckObstacleCollision())
                            {
                                _core.QueryCommand(command);
                            }
                        }
                        else
                        {
                            _core.QueryCommand(command);
                        }
                    }*/
                
                    _telloCore.QueryCommand(command);
                }

            }
        }

        /// <summary>
        /// Wurde aus dem Agenten kopiert, da die methode durch eine Signatur mit einem Action return wert ersetzt werden musste.
        /// Diese Methode eigenet sich aber für die richtige Consolensteuerun.
        /// </summary>
        private TelloAction ReadKeyboard()
        {
            TelloAction action = TelloAction.Unknown;
            var key = Console.ReadKey(true);

            if (key != null)
            {
                Console.WriteLine($"{key.Key} pressed");

                switch (key.Key)
                {
                    case ConsoleKey.W: action = TelloAction.MoveForward; break;
                    case ConsoleKey.S: action = TelloAction.MoveBackward; break;
                    case ConsoleKey.A: action = TelloAction.MoveLeft; break;
                    case ConsoleKey.D: action = TelloAction.MoveRight; break;
                    case ConsoleKey.R: action = TelloAction.Rise; break;
                    case ConsoleKey.F: action = TelloAction.Sink; break;
                    case ConsoleKey.Q: action = TelloAction.RotateLeft; break;
                    case ConsoleKey.E: action = TelloAction.RotateRight; break;
                    case ConsoleKey.Spacebar: action = TelloAction.Stop; break;
            
                    case ConsoleKey.T: action = TelloAction.TakeOff; break;
                    case ConsoleKey.L: action = TelloAction.Land; break;
                    case ConsoleKey.P: action = TelloAction.Emergency; break;
            
                    case ConsoleKey.B: action = TelloAction.Battery; break;
                    case ConsoleKey.C: action = TelloAction.Connect; break;
                    case ConsoleKey.O: Close(); break;
                    default: break;
                }
            }
            return action;
        }
        
        /// <summary>
        /// Check if the selected action is a movement.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool IsMovementAction(TelloAction action)
        {
            // TODO: In eine static Methode auslagern.
            return
                action == TelloAction.MoveForward ||
                action == TelloAction.MoveBackward ||
                action == TelloAction.MoveLeft ||
                action == TelloAction.MoveRight ||
                action == TelloAction.Rise ||
                action == TelloAction.Sink;
        }

        private bool CheckObstacleCollision()
        {
            return false;
        }
    }    
}
