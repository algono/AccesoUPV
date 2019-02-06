using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public class DSICManager : VPNManager
    {
        protected static IDictionary DSICCreationParameters;
        protected override IDictionary creationParameters { get { return DSICCreationParameters; } }

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
        protected override IDictionary creationParameters
        {
            get
            {
                IDictionary res = new Dictionary<string, object>();

                res.Add("AuthenticationMethod", "Eap");
                res.Add("EncryptionLevel", "Required");
                res.Add("TunnelType", "Sstp");

                System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
                ConfigXml.Load("Resources/UPV_Config.xml");
                res.Add("EapConfigXmlStream", ConfigXml);

                return res;
            }
        }

        public DSICManager(string name = null) : base(name)
        {

        }

    }
}
