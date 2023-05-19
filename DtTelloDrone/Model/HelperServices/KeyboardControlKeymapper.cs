using System.Diagnostics;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.HelperServices;

public static class KeyboardControlKeymapper
{
    private const string ConnectKey = "C";
    private const string DisconnectKey = "V";

    private const string MoveForwardKey = "W";
    private const string MoveBackwardKey = "S";
    private const string MoveLeftKey = "A";
    private const string MoveRightKey = "D";
    private const string RotateClockwiseKey = "E";
    private const string RotateCounterClockwiseKey = "Q";
    private const string RiseKey = "R";
    private const string SinkKey = "F";
    private const string StopSpaceKey = "Spacebar";
    
    private const string TakeOffKey = "T";
    private const string LandKey = "L";
    private const string EmergencyKey = "P";
    
    private const string BatteryKey = "B";
    
    private const string StartRecordedNavigationKey = "U";
    private const string StopRecordedNavigationKey = "I";
    private const string StopRecordingKeyboardInputKey = "Delete";
    
    /// <summary>
    /// Maps the key to a corresponding drone action.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The action.</returns>
    public static DroneAction MapKeyToAction(string key)
    {
        DroneAction action;
        
       switch (key)
        {
            case ConnectKey: action = DroneAction.Connect; break;
            case DisconnectKey: action = DroneAction.Disconnect; break;
            case MoveForwardKey: action = DroneAction.MoveForward; break;
            case MoveBackwardKey: action = DroneAction.MoveBackward; break;
            case MoveLeftKey: action = DroneAction.MoveLeft; break;
            case MoveRightKey: action = DroneAction.MoveRight; break;
            case RiseKey: action = DroneAction.Rise; break;
            case SinkKey: action = DroneAction.Sink; break;
            case RotateClockwiseKey: action = DroneAction.RotateClockwise; break;
            case RotateCounterClockwiseKey: action = DroneAction.RotateCounterClockwise; break;
            case StopSpaceKey: action = DroneAction.Stop; break;
            case TakeOffKey: action = DroneAction.TakeOff; break;
            case LandKey: action = DroneAction.Land; break;
            case EmergencyKey: action = DroneAction.EmergencyLanding; break;
            case BatteryKey: action = DroneAction.Battery; break;
            case StartRecordedNavigationKey: action = DroneAction.StartRrNavigation; break;
            case StopRecordedNavigationKey: action = DroneAction.StopRrNavigation; break; 
            case StopRecordingKeyboardInputKey: action = DroneAction.StopRecordRepeatNavigationRecording; break;
            default: action = DroneAction.Unknown; break;
        } 
        return action;
    }
}