using System;
using DigitalTwinOfUAV.Model.Layer;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;


namespace DigitalTwinOfUAV.Model.Agent;

public class UAV : IAgent<VirtuelEnvironmentLayer>
{
    public Guid ID { get; set; }
    public Position Position { get; set; }
    
    private int currentSpeed;

    private byte batteyCapacty;
    
    public void Init(VirtuelEnvironmentLayer layer)
    {
        Console.WriteLine("Funktioniert");
    }

    public void Tick()
    {
        
    }

    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
}