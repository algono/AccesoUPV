using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccesoUPV.UnitTests.VPNs
{
    /// <summary>
    /// Descripción resumida de VPNTests
    /// </summary>
    [TestClass]
    public class A_VPNTests
    {
        private static VPN VPN_UPV, VPN_DSIC;

        [TestMethod]
        public void VPN_UPVCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_UPV;
            vpn.SetNameAuto();
            // Keep to disconnect in further testing
            VPN_UPV = vpn;
            // Act and Assert
            ConnectionTests.CanBeConnected(vpn);
        }

        [TestMethod]
        public void VPN_UPVCanBeDisconnected()
            => ConnectionTests.CanBeDisconnected(VPN_UPV);

        [TestMethod]
        public void VPN_DSICCanBeConnected()
        {
            // Arrange
            IAccesoUPVService service = new AccesoUPVService();
            VPN vpn = service.VPN_DSIC;
            vpn.SetNameAuto();
            // Keep to disconnect in further testing
            VPN_DSIC = vpn;
            // Act and Assert
            ConnectionTests.CanBeConnected(vpn);
        }

        [TestMethod]
        public void VPN_DSICCanBeDisconnected()
            => ConnectionTests.CanBeDisconnected(VPN_DSIC);
    }
}
