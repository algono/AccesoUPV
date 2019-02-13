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
        private static IVPNManager VPN_UPV, VPN_DSIC;
        private static IDriveManager WDrive, DSICDrive;

        private static void Assert_Connected(IVPNManager Manager)
        {
            Assert.IsTrue(Manager.Connected);
            Assert.IsNotNull(Manager.ConnectedName);
            //Assert.IsTrue(Manager.IsReachable());
        }
        private static void Assert_Disconnected(IVPNManager Manager)
        {
            Assert.IsFalse(Manager.Connected);
            Assert.IsNull(Manager.ConnectedName);
            //Assert.IsFalse(Manager.IsReachable());
        }
        private static void CanBeConnected(IVPNManager Manager)
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
        private static void CanBeDisconnected(IVPNManager Manager)
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
        private static void Assert_Connected(IDriveManager Manager)
        {
            Assert.IsTrue(Manager.Connected);
            Assert.IsNotNull(Manager.ConnectedDrive);
            Assert.IsTrue(DriveManagerBase.GetMappedDrives().Contains(Manager.ConnectedDrive));
            Assert.IsTrue(Directory.Exists(Manager.ConnectedDrive));
        }
        private static void Assert_Disconnected(IDriveManager Manager)
        {
            Assert.IsFalse(Manager.Connected);
            Assert.IsNull(Manager.ConnectedDrive);
            Assert.IsFalse(Directory.Exists(Manager.Drive));
            Assert.IsFalse(DriveManagerBase.GetMappedDrives().Contains(Manager.Drive));
        }
        private static void CanBeConnected(IDriveManager Manager)
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
        private static void CanBeDisconnected(IDriveManager Manager)
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
            IVPNManager Manager = Service.VPN_UPV;
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
            IDriveManager Manager = Service.WDrive;
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
            IVPNManager Manager = Service.VPN_DSIC;
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
