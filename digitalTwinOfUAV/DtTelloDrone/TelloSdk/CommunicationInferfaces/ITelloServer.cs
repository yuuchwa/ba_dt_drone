using System;

namespace DtTelloDrone.RyzeSDK.Core
{
    public interface ITelloServer : IDisposable
    {
        void Listen();
        void Close();
    }
}
