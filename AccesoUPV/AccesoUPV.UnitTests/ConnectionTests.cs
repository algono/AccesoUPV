using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class ConnectionTests
    {
        private static IVPN VPN_UPV, VPN_DSIC;
        private static INetworkDrive WDrive, DSICDrive;

        private static void CanBeConnected(IVPN manager)
        {
            try
            {
                manager.Connect();
                ConnectionAsserts.Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(manager);
            }
        }
        private static void CanBeDisconnected(IVPN manager)
        {
            try
            {
                manager.Disconnect();
                ConnectionAsserts.Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(manager);
            }
        }
        private static void CanBeConnected(INetworkDrive manager)
        {
            try
            {
                manager.Connect();
                ConnectionAsserts.Assert_Connected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Disconnected(manager);
            }
        }
        private static void CanBeDisconnected(INetworkDrive manager)
        {
            try
            {
                manager.Disconnect();
                ConnectionAsserts.Assert_Disconnected(manager);
            }
            catch (OperationCanceledException)
            {
                ConnectionAsserts.Assert_Connected(manager);
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
            manager.Username = "algono";
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
