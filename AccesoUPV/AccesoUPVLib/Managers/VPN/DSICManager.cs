using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers.VPN
{
    public class DSICManager : VPNManagerBase
    {
        public override string Server
        {
            get
            {
                return Servers.VPN_DSIC;
            }
        }

        public override string TestServer
        {
            get
            {
                return Servers.PORTAL_DSIC;
            }
        }
        public DSICManager(string name = null) : base(name) { }

        protected override PowerShell CreateShell()
        {
            PowerShell shell = base.CreateShell();

            shell.AddParameter("AuthenticationMethod", "MSChapv2");
            shell.AddParameter("EncryptionLevel", "Optional");
            shell.AddParameter("L2tpPsk", "dsic");
            shell.AddParameter("TunnelType", "L2tp");
            shell.AddParameter("Force");

            return shell;
        }
    }
}
