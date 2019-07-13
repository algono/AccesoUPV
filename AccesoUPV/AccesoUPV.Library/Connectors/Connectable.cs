using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public interface Connectable
    {
        bool Connected { get; }

        void Connect();
        Task ConnectAsync();
        void Disconnect();
        Task DisconnectAsync();
    }
}