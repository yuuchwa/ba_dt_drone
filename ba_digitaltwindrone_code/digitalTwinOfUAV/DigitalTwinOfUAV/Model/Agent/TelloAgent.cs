using System;
using DigitalTwinOfUAV.Model.Layer;
using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace DigitalTwinOfUAV.Model.Agent;

public class TelloAgent : IAgent<BaseLayer>
{
    public Guid ID { get; set; }
    
    public Position Position { get; set; }
    
    public int Pitch { get; set; }
    public int Roll { get; set; }
    public int Yaw { get; set; }
    
    public int Altitude { get; set; }

    public int XSpeed { get; set; }
    public int YSpeed { get; set; }
    public int ZSpeed { get; set; }
    
    public int XVelocity { get; set; }
    public int YVelocity { get; set; }
    public int ZVelocity { get; set; }
    
    public DateTime OperationTime { get; set; }
    public int TimeOfFlight { get; set; }

    public int SerialNumber { get; set; }
    public int CurrentSpeed { get; set; }
    public byte BatteryCapacity { get; set; }
    public int WifiNumber { get; set; }
    public int SdkVersion { get; set; }

    public void Init(BaseLayer layer)
    {
        
    }

    public void Tick()
    {
        
    }

    public SpatialModalityType ModalityType { get; }
    public bool IsCollidingEntity { get; }
}