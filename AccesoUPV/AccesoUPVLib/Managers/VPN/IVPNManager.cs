using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public interface IVPNManager : IConnectionManager
    {
        string ConnectedName { get; }
        string Name { get; set; }
        string Server { get; }
        string TestServer { get; }

        bool Create();
        Task CreateAsync();
        bool Exists();
        List<PSObject> Find();
        bool IsReachable(int timeout = 500);
    }
}