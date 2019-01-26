using System.Collections.Generic;
using System.Management.Automation;

namespace AccesoUPV.Lib.Managers.VPN
{
    public static class DSICManager
    {
        public const string Server = "r1-vpn.dsic.upv.es";
        public const string TestServer = "portal-ng.dsic.cloud";

        public static readonly Dictionary<string, object> creationParameters;

        static DSICManager()
        {
            creationParameters = new Dictionary<string, object>();
            creationParameters.Add("AuthenticationMethod", "MSChapv2");
            creationParameters.Add("EncryptionLevel", "Optional");
            creationParameters.Add("L2tpPsk", "dsic");
            creationParameters.Add("TunnelType", "L2tp");
            creationParameters.Add("Force", true);
        }
        public static VPNManager Create(string name = null) => new VPNManager(Server, name, TestServer, creationParameters);

        public static List<PSObject> Find() => VPNManager.Find(Server);
    }
}
