namespace AccesoUPV.Library.Static
{
    public static partial class Utilities
    {
        #region WiFi Connections
        private const string NET_CMD = "nmcli";
        private static string ConnectWiFiCmd(string _) => $"nm enable true";
        private const string DISCONNECT_WIFI = "nm enable false";
        #endregion

    }
}
