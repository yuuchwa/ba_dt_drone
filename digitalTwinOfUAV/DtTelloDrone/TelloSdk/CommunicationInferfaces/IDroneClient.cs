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

        public Task<bool> InitDrone();

        public Task<bool> TakeOff();

        public Task<bool> Land();

        public Task<bool> StreamOn();

        public Task<bool> StreamOff();

        public Task<bool> Emergency();

        public Task<bool> StopAction();

        public Task<bool> FlyDirection(MoveDirection direction, int cm);

        public Task<bool> RotateDirection(RotationDirection direction, int degree);

        public Task<bool> Flip(FlipDirection direction);

        public Task<bool> FlyTo(int x, int y, int z, int speed);

        public Task<bool> Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed);

        public Task<bool> Speed(int speed);

        public void RemoteControl(int leftright, int forwardbackward, int updown, int yaw);

        public Task<bool> WiFi(string ssid, string pass);

        public Task<bool> Station(string ssid, string pass);

        public Task<string> GetSpeed();
        
        public Task<string> GetBattery();

        public Task<string> GetTime();

        public Task<string> GetWifi();

        public Task<string> GetSDK();

        public Task<string> GetSerialNumber();
    }
}
