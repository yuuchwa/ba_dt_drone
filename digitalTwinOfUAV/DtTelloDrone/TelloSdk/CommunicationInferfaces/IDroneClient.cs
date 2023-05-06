using System;
using System.Threading.Tasks;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.TelloSdk.CommunicationInferfaces
{
    /// <summary>
    /// Interface for tello command actions.
    /// </summary>
    public interface IDroneClient : IDisposable
    {
        bool IsConnected();

        void Connect();
        void Disconnect();
        
        public void Fly(MoveDirection direction, int speed);

        public void Rotate(RotationDirection direction, int speed);

        public Task<bool> InitDrone();

        public Task<bool> TakeOff();

        public Task<bool> Land();

        public Task<bool> StreamOn();

        public Task<bool> StreamOff();

        public Task<bool> Emergency();
        
        public Task<string> GetSpeed();
        
        public Task<string> GetBattery();

        public Task<string> GetTime();

        public Task<string> GetWifi();

        public Task<string> GetSDK();

        public Task<string> GetSerialNumber();
    }
}
