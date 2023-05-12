using System;
using System.Collections.Generic;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;
using DtTelloDrone.Shared;
using DtTelloDrone.TelloSdk.Attribute;
using DtTelloDrone.TelloSdk.DataModels;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter, IPositionable, IMessageBrokerSubscriber
{
    #region Properties and Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    private static readonly RecordRepeatNavigationRecorder ResourceManager = RecordRepeatNavigationRecorder.GetDirectoryManager();
    
    private readonly IDroneMessageBroker _droneMessageBroker = TelloMessageBroker.GetInstance();
    private readonly StateDeterminer _stateDeterminer = StateDeterminer.getStateDeterminerInstance();
    private Random _random = new();

    private LandScapeLayer _layer;
    private int _tickCount = 0;

    private Queue<DroneMessage> _messages = new();
    
    private Operation _operation = Operation.None;
    private RecordAndRepeatNavigationRepeater _recordAndRepeatNavigationRepeater;
    
    private readonly List<string> _newRecords = new();
    private RecordAndRepeatNavigationRecord _record;

    private DateTime _lastExecActionTs = DateTime.Now;
    private long _waitTime;
    
    private Position _target = null;

    public Guid ID { get; set; }
    private int _height;
    public Position Position { get; set; }
    private TelloStateParameter _prevParameters;
    private DroneState _prevDroneState;
    private DateTime _lastUpdateTs;

    #endregion

    # region Paths
    
    // In eine externe Datei auslagern, beim Startup den Pfad reinläd.
    private const string _keyboardRecordPath = "./home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/bin/Debug/net7.0/DtTelloDroneLogs/Log.2023-04-25/Session_20230425_1157/KeyboardControl.log";
    private const string _demoRessourcesPath =  "/home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/OutputResources/TestingResources/PlaybackNavigationDemos/demoFile.csv";
    
    #endregion
    
    #region Initialization Values

    [PropertyDescription (Name ="StartX")]
    public int StartX { get; set; }
    
    [PropertyDescription (Name = "StartY")]
    public int StartY { get; set; }
    
    [PropertyDescription (Name = "Bearing")]
    public double Bearing { get; set; }
    
    [PropertyDescription (Name ="Speed")]
    public int Speed { get; set; }
    
    #endregion

    #region Constants

    private const double DistanceTolerance = 0.1;
    private const double BearingTolerance = 2;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
        _recordAndRepeatNavigationRepeater = new RecordAndRepeatNavigationRepeater(_demoRessourcesPath);
        _droneMessageBroker.Subscribe(this);
        
        Logger.Trace(_layer);
        Logger.Info("Agent has been initialized");
    }

    #endregion

    #region Tick

    public void Tick()
    {
        try
        {
            if (_messages.TryDequeue(out DroneMessage message))                 
                ReadMessage(message);

            if (InSimulationMode())
            {
                
            }
            else
            {
                // digital twin mode.
                var parameters = _droneMessageBroker.GetStateParameter();

                if (parameters == null)
                    return;
                
                if (parameters.TimeStamp == _lastUpdateTs) 
                    return;

                DroneState state = _stateDeterminer.DetermineState(parameters);
            
                if (state != _prevDroneState) 
                    Logger.Trace($"Drone changed to {state}");
                
                UpdateAgentState(parameters, state);
                
                _prevParameters = parameters;
                _prevDroneState = state;
                _lastUpdateTs = parameters.TimeStamp;
            }
            
            ExecuteNextOperation();
            _tickCount++;
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
            DroneMessage command = 
                new DroneMessage( MessageTopic.DroneCommand, MessageSender.DigitalTwin,new Tuple<DroneAction, string>(DroneAction.EmergencyLanding, String.Empty));
            _droneMessageBroker.QueryMessage(command);
        }
    }
    
    #endregion

    #region Public Methods

    public void PublishMessage(DroneMessage message)
    {
        _messages.Enqueue(message);
    }

    #endregion
    
    #region Private Methods

    /// <summary>
    /// Processes flight information and updates the drone state.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="state"></param>
    private void UpdateAgentState(TelloStateParameter parameters, DroneState state)
    {
        if (_prevParameters == null)
        {
            return;
        }

        Bearing = DataMapper.MapToMarsBearing(parameters.Yaw);

        if (state == DroneState.MovingForward || 
            state == DroneState.MovingBackward ||
            state == DroneState.MovingLeft || 
            state == DroneState.MovingRight ||
            state == DroneState.Unknown)
        {
            Position = UpdatePosition(parameters);
        }
        
        _height = parameters.Height;

        Logger.Trace($"Agent moved to {Position}");
    }

    /// <summary>
    /// Update the position depending on the new parameters.
    /// </summary>
    /// <param name="state">The current drone state.</param>
    /// <param name="parameters">The new parameters</param>
    /// <returns>The new position the drone is moving to.</returns>
    private Position UpdatePosition(TelloStateParameter parameters)
    {
        double timeInterval = Math.Abs(parameters.TimeStamp.Millisecond - _prevParameters.TimeStamp.Millisecond);
        timeInterval /= 1000;
        
        // Die Negation verbessert die Lesbarkeit der Positionsänderung (x_prev, y_prev) => (x_curr,y_curr).
        // nach der Korrektur liest man:
        //  x_prev < x_curr => right,
        //  x_curr < x_prev => left
        //  y_prev < y_curr => fordwar,
        //  y_curr < y_prev => back
        double accelerationX = Math.Round(parameters.AccelerationX) * -1;       // cm/ms^2
        double accelerationY = Math.Round(parameters.AccelerationY) * -1;
        double velocityX = parameters.VelocityX;             // cm/ms
        double velocityY = parameters.VelocityY;

        if (TelloFlightMetrics.ForwardAccelerationThreshold < accelerationX &&
            accelerationX < TelloFlightMetrics.BackwardAccelerationThreshold)
        {
            accelerationX = 0;
        }
        
        if (TelloFlightMetrics.LeftAccelerationThreshold < accelerationY &&
            accelerationY < TelloFlightMetrics.RightAccelerationThreshold)
        {
            accelerationY = 0;
        }
        
        double speedX = DataMapper.CalculateSpeed(timeInterval, accelerationX, velocityX);
        double speedY = DataMapper.CalculateSpeed(timeInterval, accelerationY, velocityY);

        Vector<double> vec1 = new DenseVector(new []{speedX, 0});
        Vector<double> vec2 = new DenseVector(new []{0,speedY});

        double motionBearing = DataMapper.CalculateAngleOfTwoVectors(vec1, vec2);
        double motionBearingMars = DataMapper.MapCoordinateToMarsCoordinate(motionBearing);

        double flyDirection = DataMapper.CalculateFlyDirection(Bearing, motionBearingMars);

        double travelingDistance = (DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2)) / 30;

        Logger.Trace($"Bearing {Bearing}");
        
        if (DistanceTolerance <= travelingDistance)
        {
            Logger.Trace($"FlyDirection {flyDirection}");
            Logger.Trace($"TravelingDistance {travelingDistance}");
            Position = _layer._landScapeEnvironment.Move(this, flyDirection, travelingDistance);
        }

        return Position;
    }
        

    private void ExecuteNextOperation()
    {
        if (_operation == Operation.RecordAndRepeatNavigation)
        {
            if (InSimulationMode())
            {
                RunRecordRepeatNavigationInSimulation();
            }
            else
            {
                RunRecordRepeatNavigationAsDt();
                _recordAndRepeatNavigationRepeater.RecordExecuted();
                _record = null;
                _lastExecActionTs = DateTime.Now;
            }
        }
    }

    private void RunRecordRepeatNavigationInSimulation()
    {
        var records = _recordAndRepeatNavigationRepeater.GetAllRecords();
        foreach (var record in records)
        {
            // lese die nächsten zwei Aktionen aus
            // bestimme die dauert der Aktionen
            // Berechne bei einer bewegung die zurückgelegte Strecke in der Simulation.
            // stetze den Agenten auf die neue Position.
        }
    }

    private void RunRecordRepeatNavigationAsDt()
    {
        if (_record == null)
        {
            _record = _recordAndRepeatNavigationRepeater.GetNextRecord();
                
            if (_record == null)
            {
                _operation = Operation.None;
                Logger.Info($"Record and Repeat Navigation successfully completed at Position at X:{Position.X} Y:{Position.Y}");
                return;
            }
        }
            
        DroneAction action = _record.GetAction();
        
        var timePastSinceLastAction = DateTime.Now - _lastExecActionTs;
        long waitTime = _recordAndRepeatNavigationRepeater.GetWaitTime();
                
        Console.WriteLine($"WaitTime {waitTime} for action {action}");
        Console.WriteLine("TimePast: " + timePastSinceLastAction.TotalMilliseconds);
            
        if (action != DroneAction.NoAction && waitTime < timePastSinceLastAction.TotalMilliseconds)
        {
            bool isValid = true;
                
            DroneMessage command = new DroneMessage(MessageTopic.DroneCommand, MessageSender.DigitalTwin, new(action, Speed.ToString()));
            _droneMessageBroker.QueryMessage(command);
                
            if (action == DroneAction.Stop)
            {
                isValid = _recordAndRepeatNavigationRepeater.ValidateCheckpoint(this.Position);
                if (!isValid)
                {
                    command = new DroneMessage(MessageTopic.DroneCommand, MessageSender.DigitalTwin, new(DroneAction.Land, Speed.ToString()));
                    _droneMessageBroker.QueryMessage(command);
                    _operation = Operation.None;
                    Logger.Info($"{action} could not be executed and the operation has been aborted.");
                    Logger.Info($"The drone is not located at the intended position X:{_record.GetPosition().X} Y:{_record.GetPosition().Y}, but outside the valid range of 10cm at X:{Position.X} Y:{Position.Y}.");
                }
            }
        }   
    }

    private bool CheckObstacleCollision()
    {
        return false;
    }

    private void ReadMessage(DroneMessage message)
    {
        // Eigene Nachichten nicht empfangen
        if (message.GetSource() == MessageSender.Drone)
            return;
        
        MessageTopic topic = message.GetTopic();

        if (topic == MessageTopic.DroneCommand)
        {
            _newRecords.Add(CreateRecord(message));
        }
        else if (topic == MessageTopic.Operation)
        {
            var action = message.GetCommand().Item1;
            
            switch (action) 
            {
                case DroneAction.StartRecordRepeatNavigation:
                    Logger.Info("Record-Repeat Navigation started");
                    _operation = Operation.RecordAndRepeatNavigation; 
                    break;
                case DroneAction.StopRecordRepeatNavigation:
                    Logger.Info("Record-Repeat Navigation stopped");
                    break;
                case DroneAction.StopRecordRepeatNavigationRecording:
                    FlushRecords();
                    RecordRepeatNavigationRecorder.Close();
                    break;
                default: break;
            }
        }
    }

    private string CreateRecord(DroneMessage message)
    {
        return DateTime.Now.ToString("hhmmssfff") + ";" + 
               message.GetCommand().Item1 + ";" + 
               Math.Truncate(Position.X) + ";" + 
               Math.Truncate(Position.Y) + ";" + 
               _height + 
               "\n";
    }
    
    private void FlushRecords()
    {
        foreach (var record in _newRecords)
        {
            ResourceManager.AppendToKeyboardInputFile(record);
        }
    }
    
    private void createGPX()
    {
        // Nachkommastellen reduzieren.
        // Qgis
    }

    /// <summary>
    /// Checks whether the system is connected to the physical drone. If so, the agent is running in online mode.
    /// </summary>
    /// <returns>The status.</returns>
    private bool InSimulationMode()
    {
        return !_droneMessageBroker.DroneConnected();
    }
    
    #endregion

    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
    
    public CollisionKind? HandleCollision(ICharacter other)
    {
        return CollisionKind.Block;
    }

    public double Extent { get; set; }
}