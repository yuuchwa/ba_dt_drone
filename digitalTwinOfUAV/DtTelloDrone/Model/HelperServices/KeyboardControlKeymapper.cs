using System.Diagnostics;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.HelperServices;

public static class KeyboardControlKeymapper
{
    private const string ConnectKey = "C";

    private const string MoveForwardKey = "W";
    private const string MoveBackwardKey = "A";
    private const string MoveLeftKey = "S";
    private const string MoveRightKey = "D";
    private const string RotateClockwiseKey = "E";
    private const string RotateCounterClockwiseKey = "Q";
    private const string RiseKey = "R";
    private const string SinkKey = "F";
    private const string StopKey = "";
    private const string StopSpaceKey = "SPACEBAR";
    
    private const string TakeOffKey = "T";
    private const string LandKey = "L";
    private const string EmergencyKey = "P";
    
    private const string BatteryKey = "B";
    
    private const string SetCheckpointKey = "Z";
    private const string RemoveLastCheckpointKey = "U";
    private const string StartRecordedNavigationKey = "I";
    
    private const string CloseKey = "O";
    
    /// <summary>
    /// Maps the key to a corresponding tello action.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The action.</returns>
    public static TelloAction MapKeyToAction(string key)
    {
        TelloAction action;
        
        switch (key)
        {
            case ConnectKey: action = TelloAction.Connect; break;

            case MoveForwardKey: action = TelloAction.MoveForward; break;
            case MoveBackwardKey: action = TelloAction.MoveBackward; break;
            case MoveLeftKey: action = TelloAction.MoveLeft; break;
            case MoveRightKey: action = TelloAction.MoveRight; break;
            case RiseKey: action = TelloAction.Rise; break;
            case SinkKey: action = TelloAction.Sink; break;
            case RotateClockwiseKey: action = TelloAction.RotateLeft; break;
            case RotateCounterClockwiseKey: action = TelloAction.RotateRight; break;
            case StopSpaceKey:
            case StopKey: action = TelloAction.Stop; break;
            
            case TakeOffKey: action = TelloAction.TakeOff; break;
            case LandKey: action = TelloAction.Land; break;
            case EmergencyKey: action = TelloAction.Emergency; break;
            
            case BatteryKey: action = TelloAction.Battery; break;
                
            case SetCheckpointKey: action = TelloAction.SetCheckpoint; break;
            case RemoveLastCheckpointKey: action = TelloAction.DeleteCheckpoint; break;
            case StartRecordedNavigationKey: action = TelloAction.StartRecordedNavigation; break;
                
            default: action = TelloAction.Unknown; break;
        }
        return action;
    }
}