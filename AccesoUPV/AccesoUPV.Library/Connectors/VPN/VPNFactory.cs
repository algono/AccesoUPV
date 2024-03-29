﻿using System.Collections;
using System.Xml;

namespace AccesoUPV.Library.Connectors.VPN
{
    public static class VPNFactory
    {
        public const string
            VPN_UPV = "vpn.upv.es", WEB_UPV = "www.upv.es", IP_PREFIX_UPV = "158.42.";
        public const string
            VPN_DSIC = "r1-vpn.dsic.upv.es", PORTAL_DSIC = "portal-ng.dsic.cloud";

        public static readonly ConnectionTestByIP UPVTest = new ConnectionTestByIP(IP_PREFIX_UPV);

        #region Configs
        private static readonly IDictionary UPVCreationParameters = new Hashtable()
        {
            { "AuthenticationMethod", "Eap" },
            { "EncryptionLevel", "Required" },
            { "TunnelType", "Sstp" }
        };

        private static readonly IDictionary DSICCreationParameters = new Hashtable()
        {
            { "AuthenticationMethod", "MSChapv2" },
            { "EncryptionLevel", "Optional" },
            { "TunnelType", "L2tp" },
            { "L2tpPsk", "dsic" },
            { "Force", true }
        };

        private static readonly VPNConfig UPVConfig = new VPNConfig(VPN_UPV, WEB_UPV, UPVTest, UPVCreationParameters);
        private static readonly VPNConfig DSICConfig = new VPNConfig(VPN_DSIC, PORTAL_DSIC, new ConnectionTestByPing(PORTAL_DSIC), DSICCreationParameters);

        static VPNFactory()
        {
            // https://docs.microsoft.com/es-es/windows/client-management/mdm/eap-configuration
            XmlDocument configXml = new XmlDocument();
            configXml.Load("Resources/UPV_Config.xml");
            UPVCreationParameters.Add("EapConfigXmlStream", configXml);
        }
        #endregion

        public static VPN GetVPNToUPV(string name = null)
            => new VPN(UPVConfig, name);

        public static VPN GetVPNToDSIC(string name = null)
            => new VPN(DSICConfig, name);

    }

}
