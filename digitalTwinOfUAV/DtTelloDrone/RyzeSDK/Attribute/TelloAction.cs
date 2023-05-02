namespace DtTelloDrone.RyzeSDK.Attribute;

/// <summary>
/// The actions of the Tello drone.
/// </summary>
public enum TelloAction
{
    NoAction,
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    RotateCounterClockwise,
    RotateClockwise,
    Stop,
    TakeOff,
    Land,
    Rise,
    Sink,
    Emergency,
    Battery,
    Speed,
    Time,
    Connect,
    Unknown,
    
    SetCheckpoint,
    DeleteCheckpoint,
    StartRecordRepeatNavigation,
    StopRecordRepeatNavigation,
    
    StopRecordingKeyboardInput
}