using System;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public interface Connectable<T> : Connectable where T : EventArgs
    {
        event EventHandler<T> Connected;
        event EventHandler<T> Disconnected;
    }

    public interface Connectable
    {
        bool IsConnected { get; }

        void Connect();
        Task ConnectAsync();
        void Disconnect();
        Task DisconnectAsync();
    }

}