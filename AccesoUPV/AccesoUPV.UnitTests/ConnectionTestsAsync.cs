using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTestsAsync
    {
        private static VPN VPN_UPV, VPN_DSIC;
        private static NetworkDrive WDrive, DSICDrive;

        private static string Username => SharedData.Username;
        private static string DSICDrivePass => SharedData.DSICDrivePass;

        [TestInitialize]
        public void PromptCredentials() => SharedData.PromptCredentials();

        private static async Task CanBeConnectedAsync(VPN vpn)
        {
            try
            {
                await vpn.ConnectAsync();
                ConnectionAsserts.Assert_Connected(vpn);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(vpn);
            }
        }
        private static async Task CanBeDisconnectedAsync(VPN vpn)
        {
            try
            {
                await vpn.DisconnectAsync();
                ConnectionAsserts.Assert_Disconnected(vpn);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(vpn);
            }
        }
        private static async Task CanBeConnectedAsync(NetworkDrive drive)
        {
            try
            {
                await drive.ConnectAsync();
                ConnectionAsserts.Assert_Connected(drive);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(drive);
            }
        }
        private static async Task CanBeDisconnectedAsync(NetworkDrive drive)
        {
            try
            {
                await drive.DisconnectAsync();
                ConnectionAsserts.Assert_Disconnected(drive);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(drive);
            }
        }

        [TestMethod]
        public async Task VPN_UPVCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_UPV;
            await vpn.SetNameAutoAsync();
            // Keep to disconnect in further testing
            VPN_UPV = vpn;
            // Act and Assert
            await CanBeConnectedAsync(vpn);
        }

        [TestMethod]
        public async Task WDriveCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.WDrive;
            drive.Username = Username;
            // Keep to disconnect in further testing
            WDrive = drive;
            // Act and Assert
            await CanBeConnectedAsync(drive);
        }

        [TestMethod]
        public async Task DSICDriveCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.DSICDrive;
            drive.Username = Username;
            drive.Password = DSICDrivePass;
            // Keep to disconnect in further testing
            DSICDrive = drive;
            // Act and Assert
            await CanBeConnectedAsync(drive);
        }

        [TestMethod]
        public async Task WDriveCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(WDrive);

        [TestMethod]
        public async Task DSICDriveCanBeDisconnected()
        {
            await CanBeDisconnectedAsync(DSICDrive);
        }

        [TestMethod]
        public async Task VPN_UPVCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(VPN_UPV);

        [TestMethod]
        public async Task VPN_DSICCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_DSIC;
            await vpn.SetNameAutoAsync();
            // Keep to disconnect in further testing
            VPN_DSIC = vpn;
            // Act and Assert
            await CanBeConnectedAsync(vpn);
        }

        [TestMethod]
        public async Task VPN_DSICCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(VPN_DSIC);

    }
}
