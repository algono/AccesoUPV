using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccesoUPV.Lib
{
    public class DSICManager : VPNManager
    {
        public DSICManager(string name) : base(name, "r1-vpn.dsic.upv.es", "portal-ng.dsic.cloud") { }

        protected override PowerShell CreateShell()
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript($"Add-VpnConnection -Name \"{Name}\" -ServerAddress \"{Server}\" -AuthenticationMethod MSChapv2 -EncryptionLevel Optional -L2tpPsk dsic -RememberCredential -TunnelType L2tp -Force;");
            return shell;
        }
    }
}
