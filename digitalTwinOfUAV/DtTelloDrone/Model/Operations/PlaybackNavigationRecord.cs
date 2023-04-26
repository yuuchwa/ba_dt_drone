using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.PathPlanning;

public class PlaybackNavigationRecord
{
    public long _timpstamp;
    public TelloAction _action;

    public PlaybackNavigationRecord(long timpstamp, TelloAction action)
    {
        _timpstamp = timpstamp;
        _action = action;
    }

    public PlaybackNavigationRecord()
    {
        _timpstamp = 0;
        _action = TelloAction.NoAction;
    }
}