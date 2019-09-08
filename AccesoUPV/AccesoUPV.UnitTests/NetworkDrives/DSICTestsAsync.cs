using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AccesoUPV.UnitTests.NetworkDrives
{
    [TestClass]
    public class DSICTestsAsync
    {
        private static VPN VPN_DSIC;
        private static NetworkDrive DSICDrive;

        [ClassInitialize]
        public static void InitVPN(TestContext _)
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_DSIC;
            if (string.IsNullOrEmpty(vpn.Name)) vpn.SetNameAuto();

            // Keep to disconnect in further testing
            VPN_DSIC = vpn;

            // Disconnect any other VPNs
            DisconnectOtherVPNs(_);

            // Add VPN to list for others to disconnect it if it causes trouble for them
            SharedData.VPNs.Add(vpn);

            SharedData.PromptUsername();
            SharedData.PromptPasswordDSIC();
        }

        public static void DisconnectOtherVPNs(TestContext _)
        {
            foreach (VPN vpn in SharedData.VPNs)
            {
                if (vpn.IsConnected && !vpn.ConnectedName.Equals(VPN_DSIC.Name))
                {
                    vpn.Disconnect();
                }
            }
        }

        [TestInitialize]
        public void ConnectToVPN()
        {
            if (!VPN_DSIC.IsConnected && !VPN_DSIC.IsReachable())
            {
                VPN_DSIC.Connect();
            }
        }

        [ClassCleanup]
        public static void DisconnectVPN()
        {
            if (VPN_DSIC.IsConnected)
            {
                VPN_DSIC.Disconnect();
            }
        }

        [TestMethod]
        public async Task DSICDriveCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            NetworkDrive drive = service.Disco_DSIC;
            drive.Username = SharedData.Username;
            drive.Password = SharedData.DSICDrivePass;
            // Keep to disconnect in further testing
            DSICDrive = drive;
            // Act and Assert
            await ConnectionTestsAsync.CanBeConnectedAsync(drive);
        }

        [TestMethod]
        public async Task DSICDriveCanBeDisconnectedAsync()
            => await ConnectionTestsAsync.CanBeDisconnectedAsync(DSICDrive);
    }
}
