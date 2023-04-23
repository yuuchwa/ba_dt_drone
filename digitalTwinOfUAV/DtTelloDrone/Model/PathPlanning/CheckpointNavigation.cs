using System.Collections.Generic;
using Mars.Interfaces.Environments;

namespace DtTelloDrone.Model.PathPlanning;

public struct CheckpointNavigation
{
    private Queue<Position> Checkpoints = new Queue<Position>();

    public CheckpointNavigation()
    {
    }

    public void AddCheckpoint(Position checkpoint)
    {
        Checkpoints.Enqueue(checkpoint);
    }

    public Position RemoveLastCheckpoint()
    {
        var result = Checkpoints.TryDequeue(out var dequeuedPosition);
        
        return dequeuedPosition;
    }

    public int NumberOfCheckPoints()
    {
        return Checkpoints.Count;
    }

    public void SavecheckpointsInFile()
    {
        
    }
}