namespace DtTelloDrone.Model.Attributes;

/// <summary>
/// The actions of the Tello drone.
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
    Unknown,
    
    SetCheckpoint,
    DeleteCheckpoint,
    StartRecordRepeatNavigation,
    StopRecordRepeatNavigation,
    
    StopRecordRepeatNavigationRecording
}