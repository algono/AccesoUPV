using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccesoUPV.Library.Managers.Drive;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Managers.VPN;
using AccesoUPV.Library.Managers;
using System.IO;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTests
    {
        private static IVPN VPN_UPV, VPN_DSIC;
        private static INetworkDrive WDrive, DSICDrive;

        private static void Assert_Connected(IVPN Manager)
        {
            Assert.IsTrue(Manager.Connected);
            Manager.Refresh();
            Assert.IsTrue(Manager.Connected);
        }
        private static void Assert_Disconnected(IVPN Manager)
        {
            Assert.IsFalse(Manager.Connected);
            Manager.Refresh();
            Assert.IsFalse(Manager.Connected);
        }
        private static void CanBeConnected(IVPN Manager)
        {
            try
            {
                Manager.Connect();
                Assert_Connected(Manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Disconnected(Manager);
            }
        }
        private static void CanBeDisconnected(IVPN Manager)
        {
            try
            {
                Manager.Disconnect();
                Assert_Disconnected(Manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Connected(Manager);   
            }
        }
        private static void Assert_Connected(INetworkDrive Manager)
        {
            Assert.IsTrue(Manager.Connected);
            Assert.IsNotNull(Manager.ConnectedDrive);
            Assert.IsTrue(NetworkDriveBase.GetMappedDrives().Contains(Manager.ConnectedDrive));
            Assert.IsTrue(Directory.Exists(Manager.ConnectedDrive));
        }
        private static void Assert_Disconnected(INetworkDrive Manager)
        {
            Assert.IsFalse(Manager.Connected);
            Assert.IsNull(Manager.ConnectedDrive);
            Assert.IsFalse(Directory.Exists(Manager.Drive));
            Assert.IsFalse(NetworkDriveBase.GetMappedDrives().Contains(Manager.Drive));
        }
        private static void CanBeConnected(INetworkDrive Manager)
        {
            try
            {
                Manager.Connect();
                Assert_Connected(Manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Disconnected(Manager);
            }
        }
        private static void CanBeDisconnected(INetworkDrive Manager)
        {
            try
            {
                Manager.Disconnect();
                Assert_Disconnected(Manager);
            }
            catch (OperationCanceledException)
            {
                Assert_Connected(Manager);
            }
        }

        [TestMethod]
        public void VPN_UPVCanBeConnected()
        {
            // Arrange
            AccesoUPVService Service = new AccesoUPVService();
            IVPN Manager = Service.VPN_UPV;
            Manager.Name = "UPV";
            // Keep to disconnect in further testing
            VPN_UPV = Manager;
            // Act and Assert
            CanBeConnected(Manager);
        }

        [TestMethod]
        public void WDriveCanBeConnected()
        {
            // Arrange
            AccesoUPVService Service = new AccesoUPVService();
            INetworkDrive Manager = Service.WDrive;
            Manager.UserName = "algono";
            // Keep to disconnect in further testing
            WDrive = Manager;
            // Act and Assert
            CanBeConnected(Manager);
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
        public void WDriveCanBeDisconnected()
        {
            CanBeDisconnected(WDrive);
        }

        //[TestMethod]
        //public void DSICDriveCanBeDisconnected()
        //{
        //    CanBeDisconnected(DSICDrive);
        //}

        [TestMethod]
        public void VPN_UPVCanBeDisconnected()
        {
            CanBeDisconnected(VPN_UPV);
        }

        [TestMethod]
        public void VPN_DSICCanBeConnected()
        {
            // Arrange
            AccesoUPVService Service = new AccesoUPVService();
            IVPN Manager = Service.VPN_DSIC;
            Manager.Name = "DSIC";
            // Keep to disconnect in further testing
            VPN_DSIC = Manager;
            // Act and Assert
            CanBeConnected(Manager);
        }

        [TestMethod]
        public void VPN_DSICCanBeDisconnected()
        {
            CanBeDisconnected(VPN_DSIC);
        }

    }
}
