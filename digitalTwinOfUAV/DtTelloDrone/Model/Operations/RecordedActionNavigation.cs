using System.Collections.Generic;
using System.IO;
using System.Linq;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.PathPlanning;

public class RecordedActionNavigation
{
    private const string _path = "./home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/bin/Debug/net7.0/DtTelloDroneLogs/Log.2023-04-25/Session_20230425_1157/KeyboardControl.log";
    private const string _testFilePath = "";
    private Queue<TelloAction> _actions = new Queue<TelloAction>();

    public RecordedActionNavigation()
    {
        ReadRecordedKeyinputs();
    }

    public TelloAction GetNextAction()
    {
        var notEmpty = _actions.TryDequeue(out var action);
        if (!notEmpty)
        {
            action = TelloAction.StopRecordedNavigation;
        }
        return action;
    }
    
    private void ReadRecordedKeyinputs()
    {
        var instructions = File.ReadLines(_path)
            .Select(instruction => instruction.Split(";")).ToArray();
        
        foreach(var instruction in instructions[0])
        {
            _actions.Enqueue(KeyboardControlKeymapper.MapKeyToAction(instruction));
        }
    }
}