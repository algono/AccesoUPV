using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Connectors.VPN;

namespace AccesoUPV.UnitTests.VPNs
{
    /// <summary>
    /// Descripción resumida de VPNAsyncTests
    /// </summary>
    [TestClass]
    public class A_VPNTestsAsync
    {
        private static VPN VPN_UPV, VPN_DSIC;

        [TestMethod]
        public async Task VPN_UPVCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_UPV;
            await vpn.SetNameAutoAsync();
            // Keep to disconnect in further testing
            VPN_UPV = vpn;
            // Act and Assert
            await ConnectionTestsAsync.CanBeConnectedAsync(vpn);
        }

        [TestMethod]
        public async Task VPN_UPVCanBeDisconnectedAsync()
            => await ConnectionTestsAsync.CanBeDisconnectedAsync(VPN_UPV);

        [TestMethod]
        public async Task VPN_DSICCanBeConnectedAsync()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_DSIC;
            await vpn.SetNameAutoAsync();
            // Keep to disconnect in further testing
            VPN_DSIC = vpn;
            // Act and Assert
            await ConnectionTestsAsync.CanBeConnectedAsync(vpn);
        }

        [TestMethod]
        public async Task VPN_DSICCanBeDisconnectedAsync()
            => await ConnectionTestsAsync.CanBeDisconnectedAsync(VPN_DSIC);
    }
}
