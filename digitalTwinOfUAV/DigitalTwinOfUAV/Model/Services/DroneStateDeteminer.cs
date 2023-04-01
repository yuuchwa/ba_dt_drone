using System;
using System.Collections.Generic;
using DigitalTwinOfUAV.Model.Attributes;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Services;

/// <summary>
/// This class determines the current state of the drone.
/// </summary>
public class StateDeterminer
{
    private static StateDeterminer _stateDeterminer;
    
    private List<Func<DroneState>> _functions;
    
    private TelloStateParameter _stateParameter;
    private DroneState _prevState;
    
    public DroneState DetermineState(TelloStateParameter parameter)
    {
        _stateParameter = parameter;
        
        DroneState droneState = DroneState.Unknown;
        foreach (Func<DroneState> function in _functions)
        {
            droneState = function();

            if (droneState != DroneState.Unknown)
            {
                return droneState;
            }
        }

        return droneState;
    }

    public static StateDeterminer getStateDeterminerinstance() => _stateDeterminer ?? new StateDeterminer();

    /// <summary>
    /// Instantiate the State determiner.
    /// </summary>
    private StateDeterminer()
    {
        _functions = new()
        {
            InStandby,
            Hovering,
            TakingOff,
            Landing,
            MovingForward,
            MovingBackward,
            MovingLeft,
            MovingRight,
            Rising,
            Sinking,
            RotatingClockwise,
            RotatingCounterClockwise
        };

        _stateParameter = null;
        _prevState = DroneState.Unknown;
    }
    
    private DroneState InStandby()
    {
        if (_prevState == DroneState.Standby || _prevState == DroneState.Landing)
        {
            return _stateParameter.TOF == 0 ? DroneState.Standby : DroneState.Unknown;
        }

        return DroneState.Unknown;
    }
    
    private DroneState TakingOff()
    {
        if (_prevState == DroneState.Standby || _prevState == DroneState.TakingOff)
        {
            return _stateParameter.AccelerationZ < 5 ? DroneState.TakingOff : DroneState.Unknown;
        }

        return DroneState.Unknown;
    }

    private DroneState Landing()
    {
        return DroneState.Unknown;

    }
    
    private DroneState Hovering()
    {
        return (1 <= _stateParameter.TOF) &&
               (_stateParameter.VelocityX == 0 &&
                _stateParameter.VelocityY == 0 &&
                _stateParameter.VelocityZ == 0)
            ? DroneState.Hovering
            : DroneState.Unknown;
    }

    private DroneState MovingForward()
    {
        return DroneState.Unknown;
    }
    
    private DroneState MovingBackward()
    {
        return DroneState.Unknown;
    }
    
    private DroneState MovingLeft()
    {
        return DroneState.Unknown;
    }
    
    private DroneState MovingRight()
    {
        return DroneState.Unknown;
    }
    
    private DroneState RotatingClockwise()
    {
        return DroneState.Unknown;
    }
    
    private DroneState RotatingCounterClockwise()
    {
        return DroneState.Unknown;
    }
    
    private DroneState Rising()
    {
        return DroneState.Unknown;
    }
    
    private DroneState Sinking()
    {
        return DroneState.Unknown;
    }
}