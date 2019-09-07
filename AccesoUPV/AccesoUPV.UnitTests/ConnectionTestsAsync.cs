using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using System;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests
{
    public static class ConnectionTestsAsync
    {
        internal static async Task CanBeConnectedAsync(VPN vpn)
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
        internal static async Task CanBeDisconnectedAsync(VPN vpn)
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
        internal static async Task CanBeConnectedAsync(NetworkDrive drive)
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
        internal static async Task CanBeDisconnectedAsync(NetworkDrive drive)
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
    }
}
