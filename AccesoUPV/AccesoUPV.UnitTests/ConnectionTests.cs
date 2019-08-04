using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTests
    {
        private static VPN VPN_UPV, VPN_DSIC;
        private static NetworkDrive WDrive, DSICDrive;

        private static string Username => SharedData.Username;
        private static string DSICDrivePass => SharedData.DSICDrivePass;

        [TestInitialize]
        public void PromptCredentials() => SharedData.PromptCredentials();

        private static void CanBeConnected(VPN vpn)
        {
            try
            {
                vpn.Connect();
                ConnectionAsserts.Assert_Connected(vpn);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(vpn);
            }
        }
        private static void CanBeDisconnected(VPN vpn)
        {
            try
            {
                vpn.Disconnect();
                ConnectionAsserts.Assert_Disconnected(vpn);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(vpn);
            }
        }
        private static void CanBeConnected(NetworkDrive drive)
        {
            try
            {
                drive.Connect();
                ConnectionAsserts.Assert_Connected(drive);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(drive);
            }
        }
        private static void CanBeDisconnected(NetworkDrive drive)
        {
            try
            {
                drive.Disconnect();
                ConnectionAsserts.Assert_Disconnected(drive);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(drive);
            }
        }

        [TestMethod]
        public void VPN_UPVCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_UPV;
            vpn.SetNameAuto();
            // Keep to disconnect in further testing
            VPN_UPV = vpn;
            // Act and Assert
            CanBeConnected(vpn);
        }

        [TestMethod]
        public void WDriveCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.WDrive;
            drive.Username = Username;
            // Keep to disconnect in further testing
            WDrive = drive;
            // Act and Assert
            CanBeConnected(drive);
        }

        [TestMethod]
        public void DSICDriveCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.DSICDrive;
            drive.Username = Username;
            drive.Password = DSICDrivePass;
            // Keep to disconnect in further testing
            DSICDrive = drive;
            // Act and Assert
            CanBeConnected(drive);
        }

        [TestMethod]
        public void WDriveCanBeDisconnected() => CanBeDisconnected(WDrive);

        [TestMethod]
        public void DSICDriveCanBeDisconnected()
        {
            CanBeDisconnected(DSICDrive);
        }

        [TestMethod]
        public void VPN_UPVCanBeDisconnected() => CanBeDisconnected(VPN_UPV);

        [TestMethod]
        public void VPN_DSICCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_DSIC;
            vpn.SetNameAuto();
            // Keep to disconnect in further testing
            VPN_DSIC = vpn;
            // Act and Assert
            CanBeConnected(vpn);
        }

        [TestMethod]
        public void VPN_DSICCanBeDisconnected() => CanBeDisconnected(VPN_DSIC);

    }
}
