using System;
using System.Collections.Generic;
using RyzeTelloSDK.Models;

namespace DigitalTwinOfUAV.Model.Attributes;

public enum State
{
    Unknown,
    Standby,
    Hovering, 
    MovingForwards,
    MovingBackwards,
    MovingLeft,
    MovingRight,
    RotatingClockwise,
    RotatingCounterClockwise,
    Lifting,
    IsEmergency,
}

public class StateDeterminer
{
    private static StateDeterminer _stateDeterminer;
    private List<Func<State>> _functions = new();
    private TelloStateParameter _telloStateParameter = null;


    public State DetermineState(TelloStateParameter parameter)
    {
        _telloStateParameter = parameter;
        
        State state = State.Unknown;
        foreach (Func<State> function in _functions)
        {
            state = function();

            if (state != State.Unknown)
            {
                return state;
            }
        }

        return state;
    }

    public static StateDeterminer getStateDeterminerinstance() => _stateDeterminer ?? new StateDeterminer();

    private StateDeterminer()
    {
        _functions.Add(IsStandby);
        _functions.Add(IsHovering);
        _functions.Add(IsMovingUp);
        _functions.Add(IsMovingDown);
        _functions.Add(IsMovingForward);
        _functions.Add(IsMovingBackward);
        _functions.Add(IsMovingLeft);
        _functions.Add(IsMovingRight);
        _functions.Add(IsRotatingClockwise);
        _functions.Add(IsRotatingCounterClockwise);
    }
    
    private State IsStandby()
    {
        return _telloStateParameter.TOF == 0 ? State.Standby : State.Unknown;
    }
    
    private State IsHovering()
    {
        return (1 <= _telloStateParameter.TOF) &&
               (_telloStateParameter.VelocityX == 0 &&
                _telloStateParameter.VelocityY == 0 &&
                _telloStateParameter.VelocityZ == 0)
            ? State.Hovering
            : State.Unknown;
    }
    
    
    private State IsMovingForward()
    {
        return State.Unknown;
    }
    
    private State IsMovingBackward()
    {
        return State.Unknown;
    }
    
    private State IsMovingLeft()
    {
        return State.Unknown;
    }    
    
    private State IsMovingRight()
    {
        return State.Unknown;
    }
    
    private State IsRotatingClockwise()
    {
        return _telloStateParameter.Yaw;
    }
    
    private State IsRotatingCounterClockwise()
    {
        return State.Unknown;
    }
    
    private State IsMovingUp()
    {
        return State.Unknown;
    }
    
    private State IsMovingDown()
    {
        return State.Unknown;
    }

}