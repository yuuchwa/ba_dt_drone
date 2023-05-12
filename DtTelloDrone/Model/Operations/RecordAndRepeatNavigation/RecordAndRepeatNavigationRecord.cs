using DtTelloDrone.Model.Attributes;
using Mars.Interfaces.Environments;

namespace DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;

/// <summary>
/// This class represents a single record of the Record-Repeat Navigation.
/// </summary>
public class RecordAndRepeatNavigationRecord
{
    private readonly long _timpstamp;
    private readonly DroneAction _action;
    private readonly Position _position;
    private readonly int _height;

    /// <summary>
    /// Initialize a Record.
    /// </summary>
    /// <param name="timpstamp">The timestamp when the record is made.</param>
    /// <param name="action">the action which is recorded</param>
    /// <param name="position">The position of the agent.</param>
    /// <param name="height">The height of the agent.</param>
    public RecordAndRepeatNavigationRecord(long timpstamp, DroneAction action, Position position, int height)
    {
        _timpstamp = timpstamp;
        _action = action;
        _position = position;
        _height = height;
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <returns>The timestamp</returns>
    public long GetTimestamp()
    {
        return _timpstamp;
    }
    
    /// <summary>
    /// Gets the action.
    /// </summary>
    /// <returns>The action</returns>
    public DroneAction GetAction()
    {
        return _action;
    }
    
    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <returns>The positon</returns>
    public Position GetPosition()
    {
        return _position;
    }
    
    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <returns>The height</returns>
    public int GetHeight()
    {
        return _height;
    }
}