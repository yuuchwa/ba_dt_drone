namespace DtTelloDrone.RyzeSDK.Attribute;

/// <summary>
/// The actions of the Tello drone.
/// </summary>
public enum TelloAction
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    RotateLeft,
    RotateRight,
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
    StartAutonomousNavigation
}