﻿using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AccesoUPV.UnitTests
{
    public static class ConnectionAsserts
    {
        public static void Assert_Connected(IVPN manager)
        {
            Assert.IsTrue(manager.Connected);
            manager.CheckConnection();
            Assert.IsTrue(manager.Connected);
        }

        public static void Assert_Disconnected(IVPN manager)
        {
            Assert.IsFalse(manager.Connected);
            manager.CheckConnection();
            Assert.IsFalse(manager.Connected);
        }

        public static void Assert_Connected(INetworkDrive manager)
        {
            Assert.IsTrue(manager.Connected);
            Assert.IsNotNull(manager.ConnectedDrive);
            Assert.IsTrue(NetworkDrive.GetMappedDrives().Contains(manager.ConnectedDrive));
            Assert.IsTrue(Directory.Exists(manager.ConnectedDrive));
        }

        public static void Assert_Disconnected(INetworkDrive manager)
        {
            Assert.IsFalse(manager.Connected);
            Assert.IsNull(manager.ConnectedDrive);
            Assert.IsFalse(Directory.Exists(manager.Drive));
            Assert.IsFalse(NetworkDrive.GetMappedDrives().Contains(manager.Drive));
        }
    }
}
