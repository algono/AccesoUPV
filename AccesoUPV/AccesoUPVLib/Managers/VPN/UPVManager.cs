using System.Collections.Generic;
using System.Management.Automation;

namespace AccesoUPV.Lib.Managers.VPN
{
    public static class UPVManager
    {
        public const string Server = "vpn.upv.es";
        public const string TestServer = "www.upv.es";

        public static readonly Dictionary<string, object> creationParameters;

        static UPVManager()
        {
            creationParameters = new Dictionary<string, object>();
            creationParameters.Add("AuthenticationMethod", "Eap");
            creationParameters.Add("EncryptionLevel", "Required");
            creationParameters.Add("TunnelType", "Sstp");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            creationParameters.Add("EapConfigXmlStream", ConfigXml);
        }
        public static VPNManager Create(string name = null) => new VPNManager(Server, name, TestServer, creationParameters);

        public static List<PSObject> Find() => VPNManager.Find(Server);
    }
}
