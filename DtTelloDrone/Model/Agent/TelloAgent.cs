using System;
using System.Collections.Generic;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.Model.Layer;
using DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;
using DtTelloDrone.TelloSdk.Attribute;
using DtTelloDrone.TelloSdk.DataModels;
using Mars.Components.Environments.Cartesian;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Annotations;
using Mars.Interfaces.Environments;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DtTelloDrone.Model.Agent;

public class TelloAgent : IAgent<LandScapeLayer>, ICharacter, IMessageBrokerSubscriber
{
    #region Properties and Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;
    private static readonly RecordRepeatNavigationRecorder ResourceManager = RecordRepeatNavigationRecorder.GetRecorder();
    
    private readonly IDroneMessageBroker _droneMessageBroker = TelloMessageBroker.GetInstance();
    private readonly StateDeterminer _stateDeterminer = StateDeterminer.getStateDeterminerInstance();

    private LandScapeLayer _layer;
    private readonly  Queue<DroneMessage> _messages = new();
    
    private Operation _operation = Operation.None;
    private RecordAndRepeatNavigationRepeater _recordAndRepeatNavigationRepeater;
    
    private readonly List<string> _newRecords = new();
    private RecordAndRepeatNavigationRecord _record;

    private DateTime _lastExecActionTs = DateTime.Now;
    
    public Guid ID { get; set; }
    private int _height;
    public Position Position { get; set; }
    private TelloStateParameter _prevParameters;
    private DroneState _prevDroneState;
    private DateTime _lastUpdateTs;

    #endregion

    # region Paths
    
    // In eine externe Datei auslagern, beim Startup den Pfad läd.
    private const string DemoRessourcesPath =  "/home/leon/Documents/Studium/Bachelorarbeit/BA_DigitalTwinDrone_Code/DtTelloDrone/OutputResources/TestingResources/PlaybackNavigationDemos/BA_Exp2_Trajection.csv";
    
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
    private const double AgentSimulationSpeedScaling = 20.0;

    #endregion

    #region Initialization

    public void Init(LandScapeLayer layer)
    {
        _layer = layer;
        Position = Position.CreatePosition(StartX, StartY);
        _layer._landScapeEnvironment.Insert(this, Position);
        _recordAndRepeatNavigationRepeater = new RecordAndRepeatNavigationRepeater(DemoRessourcesPath);
        _droneMessageBroker.Subscribe(this);
        
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

                if (parameters.Battery <= TelloFlightMetrics.CriticalBatteryState)
                {
                    var emergencyMsg = new DroneMessage(
                        MessageTopic.Operation, 
                        MessageSender.DigitalTwin,
                        new(DroneAction.Stop, String.Empty));
                    _droneMessageBroker.QueryMessage(emergencyMsg);
                    
                    emergencyMsg = new DroneMessage(
                        MessageTopic.Operation, 
                        MessageSender.DigitalTwin,
                        new(DroneAction.Land, String.Empty));
                    _droneMessageBroker.QueryMessage(emergencyMsg);
                    Logger.Info("The drone was landed because its battery level is below 5%.");
                }
                
                if (parameters.TimeStamp == _lastUpdateTs) 
                    return;

                DroneState state = _stateDeterminer.DetermineState(parameters);

                UpdateAgentState(parameters, state);
                
                _prevParameters = parameters;
                _prevDroneState = state;
                _lastUpdateTs = parameters.TimeStamp;
            }
            
            ExecuteNextOperation();
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
    }

    /// <summary>
    /// Update the position depending on the new parameters.
    /// </summary>
    /// <param name="parameters">The new parameters</param>
    /// <returns>The new position the drone is moving to.</returns>
    private Position UpdatePosition(TelloStateParameter parameters)
    {
        double timeInterval = Math.Abs(parameters.TimeStamp.Millisecond - _prevParameters.TimeStamp.Millisecond);
        timeInterval /= 1000;
        
        // Acceleration negiert, um die Vorzeichen mit der Velocity anzugleichen
        double accelerationX = Math.Round(parameters.AccelerationX) * -1;       // cm/ms^2
        double accelerationY = Math.Round(parameters.AccelerationY);
        double velocityX = parameters.VelocityX;             // cm/ms
        double velocityY = parameters.VelocityY * (-1);
        
        if (TelloFlightMetrics.ForwardAccelerationThreshold < accelerationX &&
            accelerationX < TelloFlightMetrics.BackwardAccelerationThreshold)
        {
            accelerationX = 0;
        }

        if ((accelerationY < TelloFlightMetrics.RightAccelerationThresholdMax ||
            TelloFlightMetrics.RightAccelerationThreshold < accelerationY) &&
            (accelerationY < TelloFlightMetrics.LeftAccelerationThreshold ||
            TelloFlightMetrics.LeftAccelerationThresholdMax < accelerationY))
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

        double travelingDistance = (DataMapper.CalculateMagnitude(vec1) + DataMapper.CalculateMagnitude(vec2)) / AgentSimulationSpeedScaling;
        
        if (DistanceTolerance < travelingDistance)
        {
            _newRecords.Add(CreateRecord(DroneAction.Unknown));
            Position = _layer._landScapeEnvironment.Move(this, flyDirection, travelingDistance);
        }
        
        Logger.Trace("--- X-Values --- --- Y-Values ---");
        Logger.Trace($"AccX Before: {parameters.AccelerationX} AccY Before: {parameters.AccelerationY}");
        Logger.Trace($"AccX After:  {accelerationX} AccY After:  {accelerationY}");
        Logger.Trace($"VelX:        {velocityX} VelY:        {velocityY}");
        Logger.Trace($"SpeedX:      {speedX}  SpeedY:      {speedY}");
        Logger.Trace($"Bearing {Bearing}");
        Logger.Trace($"Time since last tick: {timeInterval}");
        Logger.Trace($"flyDirection:     {flyDirection}");
        Logger.Trace($"TraveledDistance: {travelingDistance}");

        return Position;
    }
        

    private void ExecuteNextOperation()
    {
        if (_operation == Operation.RecordAndRepeatNavigation)
        {
            if (InSimulationMode())
            {
                RunRecordRepeatNavigationInSimulation();
                _operation = Operation.None;
            }
            else
            {
                RunRecordRepeatNavigationAsDt();
            }
        }
    }

    private void RunRecordRepeatNavigationInSimulation()
    {
        RecordAndRepeatNavigationRecord record = _recordAndRepeatNavigationRepeater.GetNextRecord();

        while (record != null)  {
            var duration = _recordAndRepeatNavigationRepeater.GetWaitTime() / 1000.0;
            var action = record.GetAction();
            
            var simulationFlySpeed = Speed / 10;

            if (action == DroneAction.MoveForward ||
                action == DroneAction.MoveBackward || 
                action == DroneAction.MoveLeft || 
                action == DroneAction.MoveRight)
            {
                var direction = DataMapper.CalculateFlyDirection(Bearing, DataMapper.MapActionToBearing(action));
                var travelDistance = simulationFlySpeed * duration;

                Position = _layer._landScapeEnvironment.Move(this, direction, travelDistance);
            }
            else if (action == DroneAction.RotateClockwise ||
                     action == DroneAction.RotateCounterClockwise)
            {
                Bearing = (Bearing + DataMapper.CalculateRotation(action, Speed, duration)) % 360; // könnte falsch sein, weil nicht mars gerecht.
            }
            else if (action == DroneAction.TakeOff)
            {
                _height = 120;
            }
            else if (action == DroneAction.Land)
            {
                _height = 0;
            }
            else if (action == DroneAction.Rise)
            {
                _height += (int) (simulationFlySpeed * duration);
            }            
            else if (action == DroneAction.Sink)
            {
                _height = (int) ((-1) * simulationFlySpeed * duration);
            }
            Logger.Info($"{action};{Position.X};{Position.Y};{_height}");
            _newRecords.Add(CreateRecord(action));
            _recordAndRepeatNavigationRepeater.RecordExecuted();
            record = _recordAndRepeatNavigationRepeater.GetNextRecord();
            
        }
        
        Logger.Info($"Simulation Record-Repeat Navigation successfully completed");
    }

    private void RunRecordRepeatNavigationAsDt()
    {
        if (_record == null)
        {
            _record = _recordAndRepeatNavigationRepeater.GetNextRecord();
                
            if (_record == null)
            {
                _operation = Operation.None;
                Logger.Info($"Digital Twin Record-Repeat Navigation successfully completed");
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
            DroneMessage command = new DroneMessage(MessageTopic.DroneCommand, MessageSender.DigitalTwin, new(action, Speed.ToString()));
            _droneMessageBroker.QueryMessage(command);
                
            if (action == DroneAction.Stop)
            {
                bool isValid = _recordAndRepeatNavigationRepeater.ValidateCheckpoint(this.Position);
                if (!isValid)
                {
                    command = new DroneMessage(MessageTopic.DroneCommand, MessageSender.DigitalTwin, new(DroneAction.Land, Speed.ToString()));
                    //_droneMessageBroker.QueryMessage(command);
                    _operation = Operation.None;
                    Logger.Info($"{action} could not be executed and the operation has been aborted.");
                    Logger.Info($"The drone is not located at the intended position at X:{_record.GetPosition().X} Y:{_record.GetPosition().Y}, but outside the valid range of 10cm at X:{Position.X} Y:{Position.Y}.");
                    _newRecords.Add(CreateRecord(DroneAction.EmergencyLanding));
                    FlushRecords();
                    RecordRepeatNavigationRecorder.Close();
                }
            }
            
            _recordAndRepeatNavigationRepeater.RecordExecuted();
            _record = null;
            _lastExecActionTs = DateTime.Now;
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
            _newRecords.Add(CreateRecord(message.GetCommand().Item1));
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
                    _newRecords.Add(CreateRecord(action));
                    FlushRecords();
                    RecordRepeatNavigationRecorder.Close();
                    break;
            }
        }
    }

    private string CreateRecord(DroneAction action)
    {
        return DateTime.Now.ToString("mmssfff") + ";" + 
               action + ";" +
               Math.Truncate(Position.X) + ";" + 
               Math.Truncate(Position.Y) + ";" + 
               _height + ";"+
               Bearing + 
                "\n";
    }
    
    private void FlushRecords()
    {
        foreach (var record in _newRecords)
        {
            ResourceManager.AppendToKeyboardInputFile(record);
        }
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