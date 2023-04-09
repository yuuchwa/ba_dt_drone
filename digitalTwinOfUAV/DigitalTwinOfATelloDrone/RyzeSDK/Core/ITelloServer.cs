using System;

namespace DigitalTwinOfATelloDrone.RyzeSDK.Core
{
    public interface ITelloServer : IDisposable
    {
        void Listen();
        void Close();

    }
}
