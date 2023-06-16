namespace DtTelloDrone.Model.Attributes;

/// <summary>
/// This enum represents the operation the digital twin is executing.
/// </summary>
public enum Operation
{
    None,
    RecordAndRepeatNavigation,
    PointToPointNavigation,
}