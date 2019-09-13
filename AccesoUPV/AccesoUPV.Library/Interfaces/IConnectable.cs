using System;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Interfaces
{
    public interface IConnectable
    {
        bool IsConnected { get; }

        event EventHandler Connected;
        event EventHandler Disconnected;

        void Connect();
        Task ConnectAsync();
        void Disconnect();
        Task DisconnectAsync();
    }

}