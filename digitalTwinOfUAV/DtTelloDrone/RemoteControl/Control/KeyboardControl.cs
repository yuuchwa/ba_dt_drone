using System;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.Logger;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;
using NLog;
using ServiceStack;

namespace DtTelloDrone.RemoteControl.Control
{
    public class KeyboardControl
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
        
        public KeyboardControl()
        {
            Logger.Info("Keyboard Control initialized.");
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            Logger.Info("Keyboard Control terminated.");
        }

        public void Listen()
        {
            _cancellationToken = new CancellationTokenSource();
            Logger.Info("Keyboard Control started.");

            _mainloop = Task.Run(StartConsoleWorker, _cancellationToken.Token);
        }

        private async void StartConsoleWorker()
        {
            Logger.Info("Keyboard Control started.");
            DroneCommand command;
            while (true)
            {
                var key = Console.ReadKey(true).Key.ToString();;
                TelloAction selectedAction = KeyboardControlKeymapper.MapKeyToAction(key);
                
                if (selectedAction == TelloAction.Unknown) continue;
                
                Logger.Trace(key);

                command = new DroneCommand(selectedAction, _speed);
                _telloCore.QueryCommand(command);
            }
        }
    }    
}
