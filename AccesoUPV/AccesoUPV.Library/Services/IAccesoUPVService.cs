using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Services
{
    public interface IAccesoUPVService
    {
        bool AreUninitializedSettings { get; }
        NetworkDrive DSICDrive { get; }
        bool SavePasswords { get; set; }
        List<SettingsPropertyValue> UninitializedSettings { get; }
        string User { get; set; }
        VPN VPN_DSIC { get; }
        VPN VPN_UPV { get; }
        NetworkDriveUPV WDrive { get; }

        void ClearSettings();
        void SaveChanges();
        void Shutdown();
        Task ShutdownAsync(IProgress<string> progress = null);
    }
}