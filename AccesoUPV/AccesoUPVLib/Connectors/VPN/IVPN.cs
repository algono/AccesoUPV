using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers.VPN
{
    public interface IVPN : Connectable
    {
        string ConnectedName { get; }
        string Name { get; set; }
        string Server { get; }
        string TestServer { get; }

        bool Create();
        Task CreateAsync();
        bool Exists();
        List<PSObject> Find();
        bool IsReachable();
        bool IsReachable(int timeout);
        void Refresh();
    }
}