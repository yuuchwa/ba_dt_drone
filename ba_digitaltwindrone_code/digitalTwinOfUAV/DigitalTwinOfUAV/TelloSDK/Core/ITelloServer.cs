using System;

namespace RyzeTelloSDK.Core
{
    public interface ITelloServer : IDisposable
    {
        void Listen();
        void Close();
    }
}
