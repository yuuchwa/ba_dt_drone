using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.PathPlanning;

public class PlaybackNavigation
{
    private const string _path = "./home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/bin/Debug/net7.0/DtTelloDroneLogs/Log.2023-04-25/Session_20230425_1157/KeyboardControl.log";
    private const string _testFilePath = "/home/leon/Documents/Studium/Bachelorarbeit/ba_dt_drone/digitalTwinOfUAV/DtTelloDrone/OutputResources/TestingResources/TestRecordedNavigation/statusRequest.csv";
    private List<PlaybackNavigationRecord> _records = new();
    private int returnedRecordcounter = 0;

    public PlaybackNavigation()
    {
        ReadRecordedKeyinputs();
    }

    public PlaybackNavigationRecord GetNextRecord()
    {
        PlaybackNavigationRecord record = null;
        
        /*
        if (_records.Count <= returnedRecordcounter)
            record = new PlaybackNavigationRecord();
        */
        if (0 < _records.Count && returnedRecordcounter < _records.Count)
        {
            record = _records[returnedRecordcounter];
        }
        
        return record;
    }

    public long GetWaitTime()
    {
        long waitTime = 0;
        
        if (0 < returnedRecordcounter && returnedRecordcounter < _records.Count)
        {
            waitTime = _records[returnedRecordcounter]._timpstamp - _records[returnedRecordcounter - 1]._timpstamp;
        }

        return waitTime;
    }

    public void RecordExecuted()
    {
        returnedRecordcounter++;
    }

    private void ReadRecordedKeyinputs()
    {
        var instructions = File.ReadLines(_testFilePath)
            .Select(instruction => instruction.Split(";")).ToArray();

        PlaybackNavigationRecord record;
        
        for(int i = 1; i < instructions.Length; ++i)
        {
            var timestamp = Convert.ToInt64(instructions[i][0]);
            var action = KeyboardControlKeymapper.MapKeyToAction(instructions[i][1]);
            record = new PlaybackNavigationRecord(timestamp, action);

            _records.Add(record);
        }
    }
}