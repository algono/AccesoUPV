using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public class UPVAsyncTests
    {
        private static VPN VPN_UPV;
        private static NetworkDrive WDrive;

        [ClassInitialize]
        public static void InitVPN(TestContext _)
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_UPV;
            if (string.IsNullOrEmpty(vpn.Name)) vpn.SetNameAuto();

            // Keep to disconnect in further testing
            VPN_UPV = vpn;

            // Disconnect any other VPNs
            DisconnectOtherVPNs(_);

            // Add VPN to list for others to disconnect it if it causes trouble for them
            SharedData.VPNs.Add(vpn);
        }

        public static void DisconnectOtherVPNs(TestContext _)
        {
            foreach (VPN vpn in SharedData.VPNs)
            {
                if (vpn.IsConnected && !vpn.ConnectedName.Equals(VPN_UPV.Name))
                {
                    vpn.Disconnect();
                }
            }
        }

        [TestInitialize]
        public void ConnectToVPN()
        {
            if (!VPN_UPV.IsConnected && !VPN_UPV.IsReachable())
            {
                VPN_UPV.Connect();
            }
        }

        [ClassCleanup]
        public static void DisconnectVPN()
        {
            if (VPN_UPV.IsConnected)
            {
                VPN_UPV.Disconnect();
            }
        }

        [TestMethod]
        public async Task WDriveCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.Disco_W;
            drive.Username = SharedData.Username;
            // Keep to disconnect in further testing
            WDrive = drive;
            // Act and Assert
            await ConnectionTestsAsync.CanBeConnectedAsync(drive);
        }

        [TestMethod]
        public async Task WDriveCanBeDisconnectedAsync()
            => await ConnectionTestsAsync.CanBeDisconnectedAsync(WDrive);
    }
}
