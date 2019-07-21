using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public interface IVPN : Connectable
    {
        string ConnectedName { get; }
        string Name { get; set; }
        string Server { get; }
        string TestServer { get; }

        bool Any();
        Task<bool> AnyAsync();
        bool Create();
        Task<bool> CreateAsync();
        bool Exists();
        Task<bool> ExistsAsync();
        List<PSObject> Find();
        Task<List<PSObject>> FindAsync();
        List<string> FindNames();
        Task<List<string>> FindNamesAsync();
        bool IsReachable();
        bool IsReachable(int timeout);
        void CheckConnection();
        bool SetNameAuto();
        Task<bool> SetNameAutoAsync();
    }
}