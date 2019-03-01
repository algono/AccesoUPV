using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Connectors;
using System.IO;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTests
    {
        private static IVPN VPN_UPV, VPN_DSIC;
        private static INetworkDrive WDrive, DSICDrive;

        private static void Assert_Connected(IVPN manager)
        {
            Assert.IsTrue(manager.Connected);
            manager.Refresh();
            Assert.IsTrue(manager.Connected);
        }
        private static void Assert_Disconnected(IVPN manager)
        {
            Assert.IsFalse(manager.Connected);
            manager.Refresh();
            Assert.IsFalse(manager.Connected);
        }
        private static void CanBeConnected(IVPN manager)
        {
            try
            {
                manager.Connect();
                Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Disconnected(manager);
            }
        }
        private static void CanBeDisconnected(IVPN manager)
        {
            try
            {
                manager.Disconnect();
                Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Connected(manager);   
            }
        }
        private static void Assert_Connected(INetworkDrive manager)
        {
            Assert.IsTrue(manager.Connected);
            Assert.IsNotNull(manager.ConnectedDrive);
            Assert.IsTrue(NetworkDriveBase.GetMappedDrives().Contains(manager.ConnectedDrive));
            Assert.IsTrue(Directory.Exists(manager.ConnectedDrive));
        }
        private static void Assert_Disconnected(INetworkDrive manager)
        {
            Assert.IsFalse(manager.Connected);
            Assert.IsNull(manager.ConnectedDrive);
            Assert.IsFalse(Directory.Exists(manager.Drive));
            Assert.IsFalse(NetworkDriveBase.GetMappedDrives().Contains(manager.Drive));
        }
        private static void CanBeConnected(INetworkDrive manager)
        {
            try
            {
                manager.Connect();
                Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Disconnected(manager);
            }
        }
        private static void CanBeDisconnected(INetworkDrive manager)
        {
            try
            {
                manager.Disconnect();
                Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Connected(manager);
            }
        }

        [TestMethod]
        public void VPN_UPVCanBeConnected()
        {
            // Arrange
            AccesoUPVService service = new AccesoUPVService();
            IVPN manager = service.VPN_UPV;
            manager.Name = "UPV";
            // Keep to disconnect in further testing
            VPN_UPV = manager;
            // Act and Assert
            CanBeConnected(manager);
        }

        [TestMethod]
        public void WDriveCanBeConnected()
        {
            // Arrange
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive manager = Service.WDrive;
            manager.UserName = "algono";
            // Keep to disconnect in further testing
            WDrive = manager;
            // Act and Assert
            CanBeConnected(manager);
        }

        //[TestMethod]
        //public void DSICDriveCanBeConnected()
        //{
        //    // Arrange
        //    AccesoUPVService Service = new AccesoUPVService();
        //    IDriveManager Manager = Service.DSICDrive;
        //    Manager.UserName = "algono";
        //    Manager.Password = "INSERT PASSWORD HERE";
        //    // Keep to disconnect in further testing
        //    DSICDrive = Manager;
        //    // Act and Assert
        //    CanBeConnected(Manager);
        //}

        [TestMethod]
        public void WDriveCanBeDisconnected() => CanBeDisconnected(WDrive);

        //[TestMethod]
        //public void DSICDriveCanBeDisconnected()
        //{
        //    CanBeDisconnected(DSICDrive);
        //}

        [TestMethod]
        public void VPN_UPVCanBeDisconnected() => CanBeDisconnected(VPN_UPV);

        [TestMethod]
        public void VPN_DSICCanBeConnected()
        {
            // Arrange
            AccesoUPVService service = new AccesoUPVService();
            IVPN Manager = service.VPN_DSIC;
            Manager.Name = "DSIC";
            // Keep to disconnect in further testing
            VPN_DSIC = Manager;
            // Act and Assert
            CanBeConnected(Manager);
        }

        [TestMethod]
        public void VPN_DSICCanBeDisconnected() => CanBeDisconnected(VPN_DSIC);

    }
}
