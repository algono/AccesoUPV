using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPNToUPV : VPNBase
    {
        public static string
            VPN_UPV = "vpn.upv.es", WEB_UPV = "www.upv.es";

        public override string Server => VPN_UPV;

        public override string TestServer => WEB_UPV;

        public VPNToUPV(string name = null) : base(name) { }

        protected override PowerShell CreateShell()
        {
            PowerShell shell = base.CreateShell();

            shell.AddParameter("AuthenticationMethod", "Eap");
            shell.AddParameter("EncryptionLevel", "Required");
            shell.AddParameter("TunnelType", "Sstp");

            XmlDocument ConfigXml = new XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            shell.AddParameter("EapConfigXmlStream", ConfigXml);

            return shell;
        }
    }
}
