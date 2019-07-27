﻿using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTestsAsync
    {
        private static IVPN VPN_UPV, VPN_DSIC;
        private static INetworkDrive WDrive, DSICDrive;

        private static string Username, DSICDrivePass;

        [TestInitialize]
        public static void PromptCredentials()
        {
            Username = Interaction.InputBox("Username:");
            DSICDrivePass = Interaction.InputBox("Password (DSIC Drive):");
        }

        private static async Task CanBeConnectedAsync(IVPN vpn)
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
        private static async Task CanBeDisconnectedAsync(IVPN vpn)
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
        private static async Task CanBeConnectedAsync(INetworkDrive drive)
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
        private static async Task CanBeDisconnectedAsync(INetworkDrive drive)
        {
            try
            {
                // drive.Disconnect();
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
            AccesoUPVService service = new AccesoUPVService();
            IVPN vpn = service.VPN_UPV;
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
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive drive = Service.WDrive;
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
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive drive = Service.DSICDrive;
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
            AccesoUPVService service = new AccesoUPVService();
            IVPN vpn = service.VPN_DSIC;
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
