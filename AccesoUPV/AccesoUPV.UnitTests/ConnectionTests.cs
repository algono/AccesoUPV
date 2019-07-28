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
        private static INetworkDrive WDrive, DSICDrive;

        private static string Username, DSICDrivePass;

        [TestInitialize]
        public static void PromptCredentials()
        {
            Username = Interaction.InputBox("Username:");
            DSICDrivePass = Interaction.InputBox("Password (DSIC Drive):");
        }

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
        private static void CanBeConnected(INetworkDrive drive)
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
        private static void CanBeDisconnected(INetworkDrive drive)
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
            AccesoUPVService service = new AccesoUPVService();
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
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive drive = Service.WDrive;
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
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive drive = Service.DSICDrive;
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
            AccesoUPVService service = new AccesoUPVService();
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
