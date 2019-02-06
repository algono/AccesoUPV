using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public class UPVManager : VPNManager
    {
        protected static IDictionary UPVCreationParameters;
        protected override IDictionary creationParameters { get { return UPVCreationParameters; } }

        static UPVManager()
        {
            UPVCreationParameters = new Dictionary<string, object>();

            UPVCreationParameters.Add("AuthenticationMethod", "Eap");
            UPVCreationParameters.Add("EncryptionLevel", "Required");
            UPVCreationParameters.Add("TunnelType", "Sstp");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("UPVCreationParametersources/UPV_Config.xml");
            UPVCreationParameters.Add("EapConfigXmlStream", ConfigXml);
        }
        public UPVManager(string name = null) : base(Servers.VPN_UPV, Servers.WEB_UPV, name)
        {
        }
    }
}
