using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Static
{
    public static class RemoteDesktop
    {
        public static string
            LINUX_DSIC = "linuxdesktop.dsic.upv.es", WIN_DSIC = "windesktop.dsic.upv.es";
        public static void ConnectTo(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

        public static void ConnectToLinuxDesktop() => ConnectTo(LINUX_DSIC);
        public static void ConnectToWindowsDesktop() => ConnectTo(WIN_DSIC);
    }
}