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
        NetworkDrive Disco_DSIC { get; }
        List<SettingsPropertyValue> UninitializedSettings { get; }
        string User { get; set; }
        VPN VPN_DSIC { get; }
        VPN VPN_UPV { get; }
        NetworkDrive<UPVDomain> Disco_W { get; }
        NetworkDrive Asig_DSIC { get; }
        bool NotifyIcon { get; set; }
        bool StartMinimized { get; set; }

        event EventHandler<ShutdownEventArgs> ShuttingDown;

        void ClearSettings();
        bool ResetUPVConnection();
        Task<bool> ResetUPVConnectionAsync();
        void SaveChanges();
        void Shutdown();
        Task ShutdownAsync(IProgress<string> progress = null);
    }
}