namespace DtTelloDrone.Model.Attributes;

/// <summary>
/// This class represents the action within the system.
/// </summary>
public enum DroneAction
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
    EmergencyLanding,
    Battery,
    Speed,
    Time,
    Connect,
    Disconnect,
    Unknown,
    
    SetCheckpoint,
    DeleteCheckpoint,
    StartRecordRepeatNavigation,
    StopRecordRepeatNavigation,
    
    StopRecordRepeatNavigationRecording
}