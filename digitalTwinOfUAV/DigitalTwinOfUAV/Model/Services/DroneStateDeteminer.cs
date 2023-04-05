using System;
using System.Collections.Generic;
using DigitalTwinOfUAV.Model.Attributes;
using MongoDB.Bson;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Services;

/// <summary>
/// This class determines the current state of the drone.
/// </summary>
public class StateDeterminer
{
    #region Constants
    // TODO: constanten umbenennen
    private const int NotMeasureableTof = 10; // Wert für jede ungültige Messung oder eine Flughöhe unter 30 cm werden.
    private const int LowerMeasureableTof = 30; // Ab 30 cm aufwärts kann die ToF Messung erfasst werden
    private const int MaxMeasurebaleTof = 3000; // Name falsch, Max Höhe gegebenfalls noch nicht erreicht. wurde aber noch nicht überprüft.
    
    // Acceleration
    private const int LowerXAccelerationForHovering = -50;
    private const int UpperXAccelerationForHovering = 50;

    private const int LowerYAccelerationForHovering = -50;
    private const int UpperYAccelerationForHovering = 50;
    
    private const int LowerZAccelerationForHovering = -1100;
    private const int UpperZAccelerationForHovering = -950;
    
    //Velocity
    private const int RestVelocity = 0;
    
    // x < 0 -> Backward
    // 0 < x -> Forward
    private const int MinXVelocity = -30;
    private const int MaxXVelocity = 30;
    
    // y < 0 -> Right
    // 0 < y -> Left
    private const int MinYVelocity = -30;
    private const int MaxYVelocity = 30;

    // z < 0 -> Down
    // 0 < z -> Up
    private const int MinZVelocity = -30;
    private const int MaxZVelocity = 30;

    // Roll
    // 1 <= x -> Right
    // x <= -1 -> Left
    private const int RollBalanced = 0;
    private const int MinRollDegree = -179;
    private const int MaxRollDegree = 179;

    // Pitch
    // 1 <= x -> Backward
    // x <= -1 -> Forward
    private const int PitchBalanced = 0;
    private const int MinPitchDegree = -179;
    private const int MaxPitchDegree = 179;
    
    // Yaw
    // x moving to -1799 CCW
    // x moving to 1799 CW 
    private const int Yawbalanced = 0;
    private const int MinYawDegree = -179;
    private const int MaxYawDegree = 179;
    #endregion

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
            TakingOff,
            Landing,
            Hovering,
            MovingForward,
            MovingBackward,
            MovingLeft,
            MovingRight,
            Rising,
            Sinking,
            RotatingClockwise,
            RotatingCounterClockwise,
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
            (NotMeasureableTof == _stateParameter.TOF) &&
            (LowerXAccelerationForHovering <= _stateParameter.AccelerationX && _stateParameter.AccelerationX <= UpperXAccelerationForHovering &&
             LowerYAccelerationForHovering <= _stateParameter.AccelerationY && _stateParameter.AccelerationY <= UpperYAccelerationForHovering &&
             LowerZAccelerationForHovering <= _stateParameter.AccelerationZ && _stateParameter.AccelerationZ <= UpperZAccelerationForHovering) && 
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
        return NotMeasureableTof == _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityX <= MaxZVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
                ? DroneState.TakingOff 
                : DroneState.Unknown;
    }

    /// <summary>
    /// Checks if the drone is landing.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState Landing()
    {
        return 
            NotMeasureableTof == _stateParameter.TOF && 
            MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityX < RestVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
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
            LowerMeasureableTof <= _stateParameter.TOF &&
            _prevStateParameter.Yaw == _stateParameter.Yaw &&
            LowerXAccelerationForHovering <= _stateParameter.AccelerationX && _stateParameter.AccelerationX <= UpperXAccelerationForHovering &&
            LowerYAccelerationForHovering <= _stateParameter.AccelerationY && _stateParameter.AccelerationY <= UpperYAccelerationForHovering &&
            LowerZAccelerationForHovering <= _stateParameter.AccelerationZ && _stateParameter.AccelerationZ <= UpperZAccelerationForHovering && 
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
            LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityX && _stateParameter.VelocityX <= MaxXVelocity &&
             MinPitchDegree < _stateParameter.Pitch && _stateParameter.Pitch < PitchBalanced &&
            _stateParameter.Roll == RollBalanced
            ? DroneState.MovingForwards 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is moving backward.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingBackward()
    {
        return  
            LowerMeasureableTof <= _stateParameter.TOF && 
            MinXVelocity < _stateParameter.VelocityX && _stateParameter.VelocityX < RestVelocity && 
            PitchBalanced < _stateParameter.Pitch && _stateParameter.Pitch <= MaxPitchDegree
            ? DroneState.MovingBackwards 
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is moving left.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState MovingLeft()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF && 
            MinYVelocity <= _stateParameter.VelocityY && _stateParameter.VelocityY < RestVelocity &&
            MinRollDegree <= _stateParameter.Roll && _stateParameter.Roll < RollBalanced
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
            LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityY && _stateParameter.VelocityY <= MaxYVelocity &&
            RollBalanced < _stateParameter.Roll && _stateParameter.Roll <= MaxRollDegree 
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
            LowerMeasureableTof <= _stateParameter.TOF && 
            (_prevStateParameter.Yaw < _stateParameter.Yaw || 
            (_prevStateParameter.Yaw < Yawbalanced && Yawbalanced < _stateParameter.Yaw)) &&
            Yawbalanced < _stateParameter.Yaw && _stateParameter.Yaw <= MaxYawDegree
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
            LowerMeasureableTof <= _stateParameter.TOF && 
            (_stateParameter.Yaw < _prevStateParameter.Yaw || 
            (_stateParameter.Yaw < Yawbalanced && Yawbalanced < _prevStateParameter.Yaw)) &&
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
            LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityZ <= MaxZVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
            ? DroneState.Sinking
            : DroneState.Unknown;
    }
    
    /// <summary>
    /// Checks if the drone is rising up.
    /// </summary>
    /// <returns>the state of the drone</returns>
    private DroneState Rising()
    {
        return LowerMeasureableTof <= _stateParameter.TOF && 
            MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityZ < RestVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
                ? DroneState.Rising 
                : DroneState.Unknown;
    }
}