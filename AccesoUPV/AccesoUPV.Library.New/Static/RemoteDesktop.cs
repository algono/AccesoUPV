namespace AccesoUPV.Library.Static
{
    public static partial class RemoteDesktop
    {
        public static string
            LINUX_DSIC = "linuxdesktop.dsic.upv.es", WIN_DSIC = "windesktop.dsic.upv.es";

        public static void ConnectToLinuxDesktop() => ConnectTo(LINUX_DSIC);
        public static void ConnectToWindowsDesktop() => ConnectTo(WIN_DSIC);
    }
}