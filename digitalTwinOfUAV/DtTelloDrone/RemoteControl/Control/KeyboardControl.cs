using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK;
using DtTelloDrone.RyzeSDK.Attribute;
using DtTelloDrone.RyzeSDK.CommunicationInferfaces;
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
            TelloMessage command;
            string record;
            while (true)
            {
                var key = Console.ReadKey(true).Key.ToString();;

                /*
                if (key == "Delete")
                {
                    // Simulation unterbrechen.
                    Close();
                    _telloDroneMessageBroker.Close();
                }
                */
                
                TelloAction selectedAction = KeyboardControlKeymapper.MapKeyToAction(key);
                
                if (selectedAction == TelloAction.Unknown) continue;

                TelloTopic topic;
                if (selectedAction == TelloAction.StartRecordRepeatNavigation || 
                    selectedAction == TelloAction.StopRecordRepeatNavigation || 
                    selectedAction == TelloAction.StopRecordingKeyboardInput)
                    topic = TelloTopic.Operation;
                else 
                    topic = TelloTopic.DroneControl;

                command = new TelloMessage(topic, MessageSender.KeyboardControl, new(selectedAction, _speed.ToString()));

                _telloDroneMessageBroker.QueryCommand(command);
            }
        }
    }    
}
