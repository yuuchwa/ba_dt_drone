using System;
using DtTelloDrone.TelloSdk.DataModels;

namespace DtTelloDrone.TelloSdk.CommunicationInferfaces
{
    public interface IDroneServer : IDisposable
    {
        public void Listen();
        public void Close();

        public string GetRawState();
        public TelloStateParameter GetStateParameter();
    }
}
