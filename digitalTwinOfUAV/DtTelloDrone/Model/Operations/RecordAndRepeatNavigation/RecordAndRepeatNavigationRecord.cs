using DtTelloDrone.RyzeSDK.Attribute;
using Mars.Interfaces.Environments;

namespace DtTelloDrone.Model.PathPlanning;

public class RecordAndRepeatNavigationRecord
{
    private readonly long _timpstamp;
    private readonly TelloAction _action;
    private readonly Position _position;
    private readonly int _height;

    public RecordAndRepeatNavigationRecord(long timpstamp, TelloAction action, Position position, int height)
    {
        _timpstamp = timpstamp;
        _action = action;
        _position = position;
        _height = height;
    }

    public RecordAndRepeatNavigationRecord()
    {
        _timpstamp = 0;
        _action = TelloAction.NoAction;
        _position = new Position(0, 0);
        _height = 0;
    }

    public long GetTimestamp()
    {
        return _timpstamp;
    }
    
    public TelloAction GetAction()
    {
        return _action;
    }
    
    public Position GetPosition()
    {
        return _position;
    }
    
    public int GetHeight()
    {
        return _height;
    }
}