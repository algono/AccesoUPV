using System.Diagnostics;

namespace AccesoUPV.Library.Static
{
    public static partial class RemoteDesktop
    {
        public static void ConnectTo(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

    }
}