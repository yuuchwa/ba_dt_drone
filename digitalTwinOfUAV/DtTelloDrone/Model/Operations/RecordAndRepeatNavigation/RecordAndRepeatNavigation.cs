#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DtTelloDrone.Model.HelperServices;
using DtTelloDrone.Model.PathPlanning;
using Mars.Interfaces.Environments;
using ServiceStack;

namespace DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;

public class RecordAndRepeatNavigation
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;

    private readonly string _path;
    private List<RecordAndRepeatNavigationRecord?> _records = new();
    private int _returnedRecordcounter;
    private const int DeviationTolerance = 5;

    public RecordAndRepeatNavigation(string path)
    {
        _path = path;
        ReadRecordedKeyinputs();
    }

    public RecordAndRepeatNavigationRecord? GetNextRecord()
    {
        RecordAndRepeatNavigationRecord? recordAndRepeatNavigationRecord = null;
        
        if (0 < _records.Count && _returnedRecordcounter < _records.Count)
        {
            recordAndRepeatNavigationRecord = _records[_returnedRecordcounter];
        }
        
        return recordAndRepeatNavigationRecord;
    }

    public long GetWaitTime()
    {
        long waitTime = 0;
        
        if (0 < _returnedRecordcounter && _returnedRecordcounter < _records.Count)
        {
            waitTime = _records[_returnedRecordcounter].GetTimestamp() - _records[_returnedRecordcounter - 1].GetTimestamp();
        }

        return waitTime;
    }

    public bool ValidateCheckpoint(Position agentPosition)
    {
        return 
            (Math.Abs(agentPosition.X - _records[_returnedRecordcounter]!.GetPosition().X) < DeviationTolerance) && 
            (Math.Abs(agentPosition.Y - _records[_returnedRecordcounter]!.GetPosition().Y) < DeviationTolerance);
    }

    public void RecordExecuted()
    {
        _returnedRecordcounter++;
    }

    private void ReadRecordedKeyinputs()
    {
        if (File.Exists(_path))
        {
            string delimiter = ";";

            var instructions = File.ReadLines(_path)
                .Select(instruction => instruction.Split(delimiter)).ToArray();

            for(int i = 1; i < instructions.Length; ++i)
            {
                var timestamp = Convert.ToInt64(instructions[i][0]);
                var action = KeyboardControlKeymapper.MapKeyToAction(instructions[i][1]);
                var positionX = Convert.ToInt32(instructions[i][2]);
                var positionY = Convert.ToInt32(instructions[i][3]);
                var positionZ = Convert.ToInt32(instructions[i][4]);

                var horizontalPosition = new Position(positionX, positionY);

                var record = new RecordAndRepeatNavigationRecord(timestamp, action, horizontalPosition, positionZ);

                _records.Add(record);
            }   
        }
        else
        {
            Logger.Info("Record file not found");
        }
    }
}