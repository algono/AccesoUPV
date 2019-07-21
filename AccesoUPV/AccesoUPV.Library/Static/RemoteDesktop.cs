using System.Diagnostics;

namespace AccesoUPV.Library.Static
{
    public static class RemoteDesktop
    {
        public static string
            LINUX_DSIC = "linuxdesktop.dsic.upv.es", WIN_DSIC = "windesktop.dsic.upv.es";

        public static void ConnectToLinuxDesktop() => ConnectTo(LINUX_DSIC);
        public static void ConnectToWindowsDesktop() => ConnectTo(WIN_DSIC);

        public static void ConnectTo(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

    }
}