using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers.VPN
{
    public class UPVManager : VPNManagerBase
    {
        public override string Server
        {
            get
            {
                return Servers.VPN_UPV;
            }
        }

        public override string TestServer
        {
            get
            {
                return Servers.WEB_UPV;
            }
        }

        public UPVManager(string name = null) : base(name) { }

        protected override PowerShell CreateShell()
        {
            PowerShell shell = base.CreateShell();

            shell.AddParameter("AuthenticationMethod", "Eap");
            shell.AddParameter("EncryptionLevel", "Required");
            shell.AddParameter("TunnelType", "Sstp");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            shell.AddParameter("EapConfigXmlStream", ConfigXml);

            return shell;
        }
    }
}
