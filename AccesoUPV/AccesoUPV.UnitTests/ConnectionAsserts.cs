using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AccesoUPV.UnitTests
{
    public static class ConnectionAsserts
    {
        public static void Assert_Connected(VPN vpn)
        {
            Assert.IsTrue(vpn.Connected);
            vpn.CheckConnection();
            Assert.IsTrue(vpn.Connected);
        }

        public static void Assert_Disconnected(VPN vpn)
        {
            Assert.IsFalse(vpn.Connected);
            vpn.CheckConnection();
            Assert.IsFalse(vpn.Connected);
        }

        public static void Assert_Connected(NetworkDrive drive)
        {
            Assert.IsTrue(drive.Connected);
            Assert.IsNotNull(drive.ConnectedDrive);
            Assert.IsTrue(NetworkDrive.GetMappedDrives().Contains(drive.ConnectedDrive));
            Assert.IsTrue(Directory.Exists(drive.ConnectedDrive));
        }

        public static void Assert_Disconnected(NetworkDrive drive)
        {
            Assert.IsFalse(drive.Connected);
            Assert.IsNull(drive.ConnectedDrive);
            Assert.IsFalse(Directory.Exists(drive.Drive));
            Assert.IsFalse(NetworkDrive.GetMappedDrives().Contains(drive.Drive));
        }
    }
}
