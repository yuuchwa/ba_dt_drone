using System;
using System.Collections.Generic;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.TelloSdk.DataModels;
using static DtTelloDrone.Model.Services.TelloFlightMetrics;

namespace DtTelloDrone.Model.Services;

/// <summary>
/// This class determines the current state of the drone.
/// </summary>
public class StateDeterminer
{
    #region Private Member Variable

    private static StateDeterminer _stateDeterminer;
    
    private List<Func<DroneState>> _functions;
    
    private TelloStateParameter _stateParameter;
    private TelloStateParameter _prevStateParameter;
    
    private DroneState _prevState;

    #endregion
    
    /// <summary>
    /// Return an instance of StateDeterminer
    /// </summary>
    /// <returns>Instance of StateDeterminer</returns>
    public static StateDeterminer getStateDeterminerInstance() => _stateDeterminer ?? new StateDeterminer();
    
    /// <summary>
    /// Instantiate the StateDeterminer.
    /// </summary>
    private StateDeterminer()
    {
        _functions = new()
        {
            InStandby,
            Rising,
            Sinking,
            MovingForward,
            MovingBackward,
            MovingLeft,
            MovingRight,
            RotatingClockwise,
            RotatingCounterClockwise,
            TakingOff,
            Landing,
            Hovering,
        };

        _prevStateParameter = new TelloStateParameter();
        _prevState = DroneState.Unknown;
    }
    
    /// <summary>
    /// Determines the state.
    /// </summary>
    /// <param name="currState">The state parameter of the drone.</param>
    /// <returns>The drone state.</returns>
    public DroneState DetermineState(TelloStateParameter currState)
    {
        _stateParameter = currState;

        DroneState currentState = DroneState.Unknown;
        foreach (Func<DroneState> function in _functions)
        {
            var droneState = function();
            
            if (droneState != DroneState.Unknown)
            {
                currentState = droneState;
                break;
            }
        }

        _prevStateParameter = _stateParameter;
        
        return currentState;
    }

    /// <summary>
    /// Checks if the drone is in standby.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState InStandby()
    {
        return 
            (ValidNonMeasureableTof == _stateParameter.TOF) &&
            (ForwardAccelerationThreshold <= _stateParameter.AccelerationX && _stateParameter.AccelerationX <= BackwardAccelerationThreshold &&
             RightAccelerationThreshold <= _stateParameter.AccelerationY && _stateParameter.AccelerationY <= LeftAccelerationThreshold &&
             SinkAccelerationThreshold <= _stateParameter.AccelerationZ && _stateParameter.AccelerationZ <= RiseAccelerationThreshold) && 
            (_stateParameter.VelocityX == RestVelocity && 
             _stateParameter.VelocityY == RestVelocity && 
             _stateParameter.VelocityZ == RestVelocity)
                ? DroneState.Standby 
                : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is taking off
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState TakingOff()
    {
        return ValidNonMeasureableTof == _stateParameter.TOF && 
               MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityZ < RestVelocity
                ? DroneState.TakingOff 
                : DroneState.Unknown;
    }

    /// <summary>
    /// Checks if the drone is landing.
    /// </summary>c
    /// <returns>the state of the drone</returns>
    private DroneState Landing()
    {
        return 
            ValidNonMeasureableTof == _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityZ <= MaxZVelocity
            ? DroneState.Landing 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is hovering
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState Hovering()
    {
        return 
            StartMeasureableTof <= _stateParameter.TOF &&
            /*
            ForwardAccelerationThreshold <= _stateParameter.AccelerationX && _stateParameter.AccelerationX <= BackwardAccelerationThreshold &&
            RightAccelerationThreshold <= _stateParameter.AccelerationY && _stateParameter.AccelerationY <=  LeftAccelerationThreshold&&
            SinkAccelerationThreshold <= _stateParameter.AccelerationZ && _stateParameter.AccelerationZ <= RiseAccelerationThreshold && 
            */
            _stateParameter.VelocityX == RestVelocity && 
            _stateParameter.VelocityY == RestVelocity && 
            _stateParameter.VelocityZ == RestVelocity
            ? DroneState.Hovering
            : DroneState.Unknown;
    }

    /// <summary>
    /// Checks if the drone is moving forward.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingForward()
    {
        return
            StartMeasureableTof <= _stateParameter.TOF &&
            ForwardVelocity <= _stateParameter.VelocityX && _stateParameter.VelocityX <= MaxXVelocity &&
            MinPitchDegree < _stateParameter.Pitch && _stateParameter.Pitch < PitchBalanced &&
            _stateParameter.AccelerationX <= ForwardAccelerationThreshold &&
            Math.Abs(_stateParameter.AccelerationY) < Math.Abs(_stateParameter.AccelerationX)  // Diagonal Bewegungen ignorieren.
                ? DroneState.MovingForward
                : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is moving backward.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingBackward()
    {
        return  
            StartMeasureableTof <= _stateParameter.TOF && 
            MinXVelocity < _stateParameter.VelocityX && _stateParameter.VelocityX <= BackwardVelocity &&
            PitchBalanced < _stateParameter.Pitch && _stateParameter.Pitch <= MaxPitchDegree &&
            BackwardAccelerationThreshold <= _stateParameter.AccelerationX &&
            Math.Abs(_stateParameter.AccelerationY) < Math.Abs(_stateParameter.AccelerationX)
            ? DroneState.MovingBackward 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is moving left.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingLeft()
    {
        return 
            StartMeasureableTof <= _stateParameter.TOF && 
            MinYVelocity <= _stateParameter.VelocityY && _stateParameter.VelocityY <= LeftVelocity &&
            MinRollDegree <= _stateParameter.Roll && _stateParameter.Roll < RollBalanced &&
            LeftAccelerationThreshold <= _stateParameter.AccelerationY&&
            Math.Abs(_stateParameter.AccelerationX) < Math.Abs(_stateParameter.AccelerationY)
            ? DroneState.MovingLeft 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is moving right.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingRight()
    {
        return 
            StartMeasureableTof <= _stateParameter.TOF && 
            RightVelocity <= _stateParameter.VelocityY && _stateParameter.VelocityY <= MaxYVelocity &&
            RollBalanced < _stateParameter.Roll && _stateParameter.Roll <= MaxRollDegree &&
            _stateParameter.AccelerationY >= RightAccelerationThreshold &&
            Math.Abs(_stateParameter.AccelerationX) < Math.Abs(_stateParameter.AccelerationY)
            ? DroneState.MovingRight 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is rotating clockwise.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState RotatingClockwise()
    {
        return
            StartMeasureableTof <= _stateParameter.TOF && 
            (_prevStateParameter.Yaw < _stateParameter.Yaw || 
            (_prevStateParameter.Yaw < InitialYaw && InitialYaw < _stateParameter.Yaw)) &&
            InitialYaw < _stateParameter.Yaw && _stateParameter.Yaw <= MaxYawDegree
            ? DroneState.RotatingClockwise 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is rotating counter clockwise.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState RotatingCounterClockwise()
    {
        return
            StartMeasureableTof <= _stateParameter.TOF && 
            (_stateParameter.Yaw < _prevStateParameter.Yaw || 
            (_stateParameter.Yaw < InitialYaw && InitialYaw < _prevStateParameter.Yaw)) &&
            MinYawDegree < _stateParameter.Yaw && _stateParameter.Yaw < MaxYawDegree
            ? DroneState.RotatingCounterClockwise 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is sinking down.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState Sinking()
    {
        return 
            StartMeasureableTof <= _stateParameter.TOF && 
            SinkVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityZ <= MaxZVelocity &&
            _stateParameter.AccelerationZ <= SinkAccelerationThreshold 
            ? DroneState.Sinking
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is rising up.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState Rising()
    {
        return StartMeasureableTof <= _stateParameter.TOF && 
            MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityZ < RiseVelocity &&
            RiseAccelerationThreshold <=_stateParameter.AccelerationZ
            ? DroneState.Rising 
            : DroneState.Unknown;
    }
}