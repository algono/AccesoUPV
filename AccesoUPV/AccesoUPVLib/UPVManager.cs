using System;
using System.Management.Automation;

namespace AccesoUPV.Lib
{
    public class UPVManager : VPNManager
    {
        public UPVManager(string name) : base(name, "vpn.upv.es", "www.upv.es") { }

        protected override PowerShell CreateShell()
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript($"param([System.Xml.XmlDocument]$importXml) Add-VpnConnection -Name \"{Name}\" -ServerAddress \"{Server}\" -AuthenticationMethod Eap -EncryptionLevel Required -RememberCredential -TunnelType Sstp -EapConfigXmlStream $importXml;");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            shell.AddParameter("importXml", ConfigXml);

            return shell;
        }
    }
}
