using System.Collections.Generic;
using System.IO;
using System.Linq;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.PathPlanning;

public class RecordedActionNavigation
{
    private const string _path = "/home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/bin/Debug/net7.0/DtTelloDroneLogs/Log.2023-04-24/Session_20230424_2201/KeyboardControl.log";
    
    private void ReadRecordedKeyinputs()
    {
        var instructions = File.ReadLines(_path)
            .Select(instruction => instruction.Split(";")).ToArray();
        
        foreach(var instruction in instructions[0])
        {
            _recordedActions.Insert(_recordedActions.Count,KeyboardControlKeymapper.MapKeyToAction(instruction));
        }
    }
}