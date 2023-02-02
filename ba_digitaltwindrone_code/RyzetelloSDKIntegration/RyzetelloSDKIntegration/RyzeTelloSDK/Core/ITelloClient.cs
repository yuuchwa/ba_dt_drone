using System;
using System.Threading.Tasks;

namespace RyzeTelloSDK.Core
{
    public interface ITelloClient : IDisposable
    {
        bool IsConnected();

        void Connect();
        void Disconnect();

        Task<string> SendCommand(string command);
        Task<bool> SendAction(string command);
    }
}
