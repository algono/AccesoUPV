using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers
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