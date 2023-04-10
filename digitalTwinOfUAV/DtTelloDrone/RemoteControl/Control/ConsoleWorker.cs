using System;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.Logger;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;
using NLog;
using ServiceStack;

namespace DtTelloDrone.RemoteControl.Control
{
    public class ConsoleWorker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Core Service
        /// </summary>
        private readonly ICore _telloCore = TelloCore.GetInstance();

        private Task _mainloop;

        private int _speed = 50;

        /// <summary>
        /// Token for canceling a thread.
        /// </summary>
        private CancellationTokenSource _cancellationToken;
        
        public ConsoleWorker()
        {
            Logger.Info("Console Control initialized.");
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            Logger.Info("Console Control terminated.");
        }

        public void Listen()
        {
            _cancellationToken = new CancellationTokenSource();
            Logger.Info("Console Control started.");

            _mainloop = Task.Run(StartConsoleWorker, _cancellationToken.Token);
        }

        private async void StartConsoleWorker()
        {
            Logger.Info("Console Control started.");
            DroneCommand command;
            while (true)
            {
                TelloAction selectedAction = ReadKeyboard();
                
                if (selectedAction == TelloAction.Unknown)
                {
                    continue;
                }

                command = new DroneCommand(selectedAction, _speed);
                _telloCore.QueryCommand(command);
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
                default:  break;
            }
            return action;
        }
    }    
}
