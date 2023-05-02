using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.Core;
using DtTelloDrone.Shared;
using Mars.Components.Starter;

namespace DtTelloDrone.RemoteControl.Control
{
    public class KeyboardControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly ResourceDirectoryManager _directoryManager =
            ResourceDirectoryManager.GetDirectoryManager();

        private SimulationStarter _simulation;
        
        private readonly List<string> _records = new();
        
        /// <summary>
        /// Core Service
        /// </summary>
        private readonly IDroneMessageBroker _telloDroneMessageBroker = TelloMessageBroker.GetInstance();

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

            foreach (var record in _records)
            {
                _directoryManager.AppendToKeyboardInputFile(record);
            }
            
            Logger.Info("Keyboard Control terminated.");
        }

        public void AddSimulation(SimulationStarter simulation)
        {
            _simulation = simulation;
        }

        public void StartKeyboardControl()
        {
            _cancellationToken = new CancellationTokenSource();
            Logger.Info("Keyboard Control started.");

            _mainloop = Task.Run(StartConsoleWorker, _cancellationToken.Token);
        }

        private async void StartConsoleWorker()
        {
            Logger.Info("Keyboard Control started.");
            DroneCommand command;
            string record;
            while (true)
            {
                var key = Console.ReadKey(true).Key.ToString();;
                record = DateTime.Now.ToString("hhmmssfff") + ";" + key + "\n";
                _records.Add(record);

                if (key == "Delete")
                {
                    // Simulation unterbrechen.
                    Close();
                    _telloDroneMessageBroker.Close();
                    ResourceDirectoryManager.Close();
                }
                
                TelloAction selectedAction = KeyboardControlKeymapper.MapKeyToAction(key);
                
                if (selectedAction == TelloAction.Unknown) continue;
                
                command = new DroneCommand(selectedAction, _speed);
                _telloDroneMessageBroker.QueryCommand(command);
            }
        }
    }    
}
