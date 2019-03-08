using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPNToDSIC : VPNBase, Openable
    {
        public static string VPN_DSIC = "r1-vpn.dsic.upv.es", PORTAL_DSIC = "portal-ng.dsic.cloud";

        public override string Server => VPN_DSIC;

        public override string TestServer => PORTAL_DSIC;
        public VPNToDSIC(string name = null) : base(name) { }

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

        public void Open()
        {
            System.Diagnostics.Process.Start("http://" + PORTAL_DSIC);
        }
    }
}
