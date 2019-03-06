using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Connectors;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTestsAsync
    {
        private static IVPN VPN_UPV, VPN_DSIC;
        private static INetworkDrive WDrive, DSICDrive;

        private static async Task CanBeConnectedAsync(IVPN manager)
        {
            try
            {
                await manager.ConnectAsync();
                ConnectionAsserts.Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(manager);
            }
        }
        private static async Task CanBeDisconnectedAsync(IVPN manager)
        {
            try
            {
                await manager.DisconnectAsync();
                ConnectionAsserts.Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(manager);   
            }
        }
        private static async Task CanBeConnectedAsync(INetworkDrive manager)
        {
            try
            {
                await manager.ConnectAsync();
                ConnectionAsserts.Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(manager);
            }
        }
        private static async Task CanBeDisconnectedAsync(INetworkDrive manager)
        {
            try
            {
                // manager.Disconnect();
                await manager.DisconnectAsync();
                ConnectionAsserts.Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(manager);
            }
        }

        [TestMethod]
        public async Task VPN_UPVCanBeConnectedAsync()
        {
            // Arrange
            AccesoUPVService service = new AccesoUPVService();
            IVPN manager = service.VPN_UPV;
            manager.Name = "UPV";
            // Keep to disconnect in further testing
            VPN_UPV = manager;
            // Act and Assert
            await CanBeConnectedAsync(manager);
        }

        [TestMethod]
        public async Task WDriveCanBeConnectedAsync()
        {
            // Arrange
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive manager = Service.WDrive;
            manager.UserName = "algono";
            // Keep to disconnect in further testing
            WDrive = manager;
            // Act and Assert
            await CanBeConnectedAsync(manager);
        }

        //[TestMethod]
        //public void DSICDriveCanBeConnectedAsync()
        //{
        //    // Arrange
        //    AccesoUPVService Service = new AccesoUPVService();
        //    IDriveManager Manager = Service.DSICDrive;
        //    Manager.UserName = "algono";
        //    Manager.Password = "INSERT PASSWORD HERE";
        //    // Keep to disconnect in further testing
        //    DSICDrive = Manager;
        //    // Act and Assert
        //    CanBeConnectedAsync(Manager);
        //}

        [TestMethod]
        public async Task WDriveCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(WDrive);

        //[TestMethod]
        //public void DSICDriveCanBeDisconnected()
        //{
        //    CanBeDisconnected(DSICDrive);
        //}

        [TestMethod]
        public async Task VPN_UPVCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(VPN_UPV);

        [TestMethod]
        public async Task VPN_DSICCanBeConnectedAsync()
        {
            // Arrange
            AccesoUPVService service = new AccesoUPVService();
            IVPN Manager = service.VPN_DSIC;
            Manager.Name = "DSIC";
            // Keep to disconnect in further testing
            VPN_DSIC = Manager;
            // Act and Assert
            await CanBeConnectedAsync(Manager);
        }

        [TestMethod]
        public async Task VPN_DSICCanBeDisconnectedAsync() => await CanBeDisconnectedAsync(VPN_DSIC);

    }
}
