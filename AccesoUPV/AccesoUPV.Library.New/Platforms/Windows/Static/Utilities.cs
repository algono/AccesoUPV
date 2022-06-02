using System.Management.Automation;

namespace AccesoUPV.Library.Static
{
    public static partial class Utilities
    {
        #region VPN
        private const string PSNameProperty = "Name";

        public static string GetStringPropertyValue(this PSObject obj, string propertyName) => (string)obj.Properties[propertyName].Value;

        public static string GetName(this PSObject obj) => obj.GetStringPropertyValue(PSNameProperty);
        #endregion

        #region WiFi Connections
        private const string NET_CMD = "netsh";
        private static string ConnectWiFiCmd(string connectionName) => $"wlan connect {connectionName}";
        private const string DISCONNECT_WIFI = "wlan disconnect";
        #endregion

    }
}
