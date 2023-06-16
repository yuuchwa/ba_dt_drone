using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DtTelloDrone.Model.Attributes;
using Mars.Interfaces.Environments;

using static DtTelloDrone.TelloSdk.Attribute.TelloFlightMetrics;

namespace DtTelloDrone.Model.Operations.RecordAndRepeatNavigation;

/// <summary>
/// This class enables the drone to record and repeat navigation paths.
/// </summary>
public class RecordAndRepeatNavigationRepeater
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger() ;

    private readonly string _path;
    private List<RecordAndRepeatNavigationRecord?> _records = new();
    private int _returnedRecordcounter;

    public RecordAndRepeatNavigationRepeater(string path)
    {
        _path = path;
        ReadRecordedKeyinputs();
    }

    public List<RecordAndRepeatNavigationRecord?> GetAllRecords()
    {
        return _records;
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
            (Math.Abs(agentPosition.X - _records[_returnedRecordcounter]!.GetPosition().X) < DeviationRadius) && 
            (Math.Abs(agentPosition.Y - _records[_returnedRecordcounter]!.GetPosition().Y) < DeviationRadius);
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
                
                if (!Enum.TryParse<DroneAction>(instructions[i][1], out var action))
                {
                    Logger.Error($"{instructions[i][1]} not an Action in .csv file");
                    continue;
                }
                
                if(action == DroneAction.Unknown)
                    continue;
                
                var timestamp = Convert.ToInt64(instructions[i][0]);

                var positionX = Convert.ToDouble(instructions[i][2]);
                var positionY = Convert.ToDouble(instructions[i][3]);
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