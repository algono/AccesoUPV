using System.Collections.Generic;
using System.Configuration;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;

namespace AccesoUPV.Library.Services
{
    public interface IAccesoUPVService
    {
        bool AreUninitializedSettings { get; }
        NetworkDriveDSIC DSICDrive { get; }
        bool SavePasswords { get; set; }
        List<SettingsPropertyValue> UninitializedSettings { get; }
        string User { get; set; }
        VPNToDSIC VPN_DSIC { get; }
        VPNToUPV VPN_UPV { get; }
        NetworkDriveW WDrive { get; }

        void SaveChanges();
    }
}