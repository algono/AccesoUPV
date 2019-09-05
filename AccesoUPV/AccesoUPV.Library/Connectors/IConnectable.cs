using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public interface IConnectable : INotifyPropertyChanged
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