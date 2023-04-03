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
    private const int MaxMeasurebaleTof = 3000; // Name falsch, Max Höhe gegebenfalls noch nicht erreich sit.
    
    // Acceleration
    private const int LowerXAccelerationForHovering = -50;
    private const int UpperXAccelerationForHovering = 50;

    private const int LowerYAccelerationForHovering = -50;
    private const int UpperYAccelerationForHovering = 50;
    
    private const int LowerZAccelerationForHovering = -1020;
    private const int UpperZAccelerationForHovering = -975;
    
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
    private const int MinRollDegree = -1799;
    private const int MaxRollDegree = 1799;

    // Pitch
    // 1 <= x -> Backward
    // x <= -1 -> Forward
    private const int PitchBalanced = 0;
    private const int MinPitchDegree = -1799;
    private const int MaxPitchDegree = 1799;
    
    // Yaw
    // x moving to -1799 CCW
    // x moving to 1799 CW 
    private const int Yawbalanced = 0;
    private const int MinYawDegree = -1799;
    private const int MaxYawDegree = 1799;
    #endregion

    #region Private Member Variable

    private static StateDeterminer _stateDeterminer;
    
    private List<Func<DroneState>> _functions;
    
    private TelloStateParameter _stateParameter;
    private TelloStateParameter _prevStateParameter;
    
    private DroneState _prevState;

    #endregion
    
    
    public DroneState DetermineState(TelloStateParameter currState)
    {
        _stateParameter = currState;
        
        DroneState droneState = DroneState.Unknown;
        foreach (Func<DroneState> function in _functions)
        {
            if(function.Equals(RotatingClockwise()) || function.Equals(RotatingCounterClockwise()) && _prevStateParameter == null) 
            {
                continue;
            }
            
            droneState = function();

            if (droneState != DroneState.Unknown)
            {
                return droneState;
            }
        }

        _prevStateParameter = _stateParameter;
        
        return droneState;
    }

    public static StateDeterminer getStateDeterminerInstance() => _stateDeterminer ?? new StateDeterminer();

    /// <summary>
    /// Instantiate the State determiner.
    /// </summary>
    private StateDeterminer()
    {
        _functions = new()
        {
            //InStandby,
            //Hovering,
            //TakingOff,
            //Landing,
            //MovingForward,
            //MovingBackward,
            MovingLeft,
            //MovingRight,
            //Rising,
            //Sinking,
            //RotatingClockwise,
            //RotatingCounterClockwise
        };

        _stateParameter = null;
        _prevState = DroneState.Unknown;
    }
    
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
    
    private DroneState TakingOff()
    {
        return NotMeasureableTof == _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityX <= MaxZVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
                ? DroneState.TakingOff 
                : DroneState.Unknown;
    }

    private DroneState Landing()
    {
        return NotMeasureableTof == _stateParameter.TOF && 
            MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityX < RestVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
            ? DroneState.Landing 
            : DroneState.Unknown;
    }
    
    private DroneState Hovering()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF &&
            LowerXAccelerationForHovering <= _stateParameter.AccelerationX && _stateParameter.AccelerationX <= UpperXAccelerationForHovering &&
             LowerYAccelerationForHovering <= _stateParameter.AccelerationY && _stateParameter.AccelerationY <= UpperYAccelerationForHovering &&
             LowerZAccelerationForHovering <= _stateParameter.AccelerationZ && _stateParameter.AccelerationZ <= UpperZAccelerationForHovering && 
            _stateParameter.VelocityX == RestVelocity && 
             _stateParameter.VelocityY == RestVelocity && 
             _stateParameter.VelocityZ == RestVelocity
            ? DroneState.Hovering
            : DroneState.Unknown;
    }

    private DroneState MovingForward()
    {
        return 
                        LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityX <= MaxZVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
            ? DroneState.Rising 
            : DroneState.Unknown;
    }
    
    private DroneState MovingBackward()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF && 
            MinXVelocity < _stateParameter.VelocityX && _stateParameter.VelocityX < RestVelocity && 
            PitchBalanced < _stateParameter.Pitch && _stateParameter.Pitch <= MaxPitchDegree
            ? DroneState.MovingBackwards 
            : DroneState.Unknown;
    }
    
    private DroneState MovingLeft()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF && 
            MinYVelocity <= _stateParameter.VelocityY && _stateParameter.VelocityY < RestVelocity &&
            MinRollDegree <= _stateParameter.Roll && _stateParameter.Roll < RollBalanced
            ? DroneState.MovingLeft 
            : DroneState.Unknown;
    }
    
    private DroneState MovingRight()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityY && _stateParameter.VelocityY <= MaxYVelocity &&
            RollBalanced < _stateParameter.Roll && _stateParameter.Roll <= MaxRollDegree 
            ? DroneState.MovingRight 
            : DroneState.Unknown;
    }
    
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
    
    private DroneState Rising()
    {
        return 
            LowerMeasureableTof <= _stateParameter.TOF && 
            RestVelocity < _stateParameter.VelocityZ && _stateParameter.VelocityX <= MaxZVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
            ? DroneState.Rising 
            : DroneState.Unknown;
    }
    
    private DroneState Sinking()
    {
        return LowerMeasureableTof <= _stateParameter.TOF && 
            MinZVelocity <= _stateParameter.VelocityZ && _stateParameter.VelocityX < RestVelocity &&
            _stateParameter.Roll == RollBalanced &&
            _stateParameter.Pitch == PitchBalanced 
                ? DroneState.Sinking 
                : DroneState.Unknown;
    }
}