using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Attributes;
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
        private static readonly RecordRepeatNavigationRecorder _directoryManager =
            RecordRepeatNavigationRecorder.GetDirectoryManager();

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

        /// <summary>
        /// Starts the thread for the keyboard control
        /// </summary>
        public void StartKeyboardControl()
        {
            _cancellationToken = new CancellationTokenSource();
            Logger.Info("Keyboard Control started.");

            _mainloop = Task.Run(StartConsoleWorker, _cancellationToken.Token);
        }

        /// <summary>
        /// Starts a worker thread to listen for keyboard inputs and map them to drone actions.
        /// </summary>
        private void StartConsoleWorker()
        {
            Logger.Info("Keyboard Control started.");
            DroneMessage command;
            string record;
            while (true)
            {
                var key = Console.ReadKey(true).Key.ToString();;

                DroneAction selectedAction = KeyboardControlKeymapper.MapKeyToAction(key);
                
                if (selectedAction == DroneAction.Unknown) continue;

                MessageTopic topic;
                if (selectedAction == DroneAction.StartRecordRepeatNavigation || 
                    selectedAction == DroneAction.StopRecordRepeatNavigation || 
                    selectedAction == DroneAction.StopRecordRepeatNavigationRecording)
                    topic = MessageTopic.Operation;
                else 
                    topic = MessageTopic.DroneCommand;

                command = new DroneMessage(topic, MessageSender.RemoteControl, new(selectedAction, _speed.ToString()));

                _telloDroneMessageBroker.QueryMessage(command);
            }
        }
    }    
}
