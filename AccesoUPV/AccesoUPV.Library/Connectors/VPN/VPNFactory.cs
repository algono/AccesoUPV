using System.Collections;
using System.Xml;

namespace AccesoUPV.Library.Connectors.VPN
{
    public static class VPNFactory
    {
        public const string
            VPN_UPV = "vpn.upv.es", WEB_UPV = "www.upv.es";
        public const string
            VPN_DSIC = "r1-vpn.dsic.upv.es", PORTAL_DSIC = "portal-ng.dsic.cloud";

        private static readonly IDictionary UPVConfig = new Hashtable()
        {
            { "AuthenticationMethod", "Eap" },
            { "EncryptionLevel", "Required" },
            { "TunnelType", "Sstp" }
        };

        private static readonly IDictionary DSICConfig = new Hashtable()
        {
            { "AuthenticationMethod", "MSChapv2" },
            { "EncryptionLevel", "Optional" },
            { "TunnelType", "L2tp" },
            { "L2tpPsk", "dsic" },
            { "Force", true }
        };

        static VPNFactory()
        {
            XmlDocument ConfigXml = new XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            UPVConfig.Add("EapConfigXmlStream", ConfigXml);
        }

        public static VPN GetVPNToUPV(string name = null)
            => new VPN(VPN_UPV, name)
            {
                TestServer = WEB_UPV,
                Config = UPVConfig
            };

        public static VPN GetVPNToDSIC(string name = null)
            => new VPN(VPN_DSIC, name)
            {
                TestServer = PORTAL_DSIC,
                Config = DSICConfig
            };
    }
}
