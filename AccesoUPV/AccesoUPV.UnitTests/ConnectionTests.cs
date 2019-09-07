using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccesoUPV.UnitTests
{
    public static class ConnectionTests
    {
        internal static void CanBeConnected(VPN vpn)
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
        internal static void CanBeDisconnected(VPN vpn)
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
        internal static void CanBeConnected(NetworkDrive drive)
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
        internal static void CanBeDisconnected(NetworkDrive drive)
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
    }
}
