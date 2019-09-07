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
            Assert.IsTrue(vpn.IsConnected);
            vpn.CheckConnection();
            Assert.IsTrue(vpn.IsConnected);
        }

        public static void Assert_Disconnected(VPN vpn)
        {
            Assert.IsFalse(vpn.IsConnected);
            vpn.CheckConnection();
            Assert.IsFalse(vpn.IsConnected);
        }

        public static void Assert_Connected(NetworkDrive drive)
        {
            Assert.IsTrue(drive.IsConnected);
            Assert.IsTrue(DriveLetterTools.IsValid(drive.ConnectedLetter));
            Assert.IsNotNull(drive.ConnectedDriveLetter);
            Assert.IsTrue(NetworkDrive.GetMappedDrives().Contains(drive.ConnectedLetter));
            Assert.IsTrue(Directory.Exists(drive.ConnectedDriveLetter));
        }

        public static void Assert_Disconnected(NetworkDrive drive)
        {
            Assert.IsFalse(drive.IsConnected);
            Assert.IsNull(drive.ConnectedDriveLetter);
            Assert.AreEqual(drive.ConnectedLetter, default);
            Assert.IsFalse(Directory.Exists(drive.DriveLetter));
            Assert.IsFalse(NetworkDrive.GetMappedDrives().Contains(drive.Letter));
        }
    }
}
