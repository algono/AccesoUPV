using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers
{
    public interface IConnectionManager
    {
        bool Connected { get; }

        void Connect();
        Task ConnectAsync();
        void Disconnect();
        Task DisconnectAsync();
    }
}