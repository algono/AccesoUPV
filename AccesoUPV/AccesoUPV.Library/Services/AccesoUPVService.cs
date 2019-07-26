﻿using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Properties;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Services
{
    public class AccesoUPVService : IAccesoUPVService
    {
        public IVPN VPN_UPV { get; }
        public IVPN VPN_DSIC { get; }
        public NetworkDriveW WDrive { get; }
        public NetworkDriveDSIC DSICDrive { get; }

        private string _user;

        public string User
        {
            get => _user;
            set
            {
                _user = value;
                WDrive.UserName = value;
                DSICDrive.UserName = value;
            }
        }
        public bool SavePasswords { get; set; }

        public bool AreUninitializedSettings => UninitializedSettings.Count > 0;
        public List<SettingsPropertyValue> UninitializedSettings { get; } = new List<SettingsPropertyValue>();

        public AccesoUPVService()
        {
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;

            _user = Settings.Default.User;

            VPN_UPV = VPNFactory.GetVPNToUPV(Settings.Default.VPN_UPVName);
            VPN_DSIC = VPNFactory.GetVPNToDSIC(Settings.Default.VPN_DSICName);

            WDrive = new NetworkDriveW(User, Settings.Default.WDriveLetter, (UPVDomain)Settings.Default.WDriveDomain);

            DSICDrive = new NetworkDriveDSIC(User, Settings.Default.DSICDrivePassword, Settings.Default.DSICDriveLetter);
            SavePasswords = DSICDrive.Password != null;
        }

        protected virtual void Default_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            foreach (SettingsPropertyValue property in Settings.Default.PropertyValues)
            {
                if (string.IsNullOrEmpty(property.PropertyValue as string))
                {
                    UninitializedSettings.Add(property);
                }
            }
        }

        public void SaveChanges()
        {
            Settings.Default.User = User;

            Settings.Default.VPN_UPVName = VPN_UPV.Name;
            Settings.Default.VPN_DSICName = VPN_DSIC.Name;

            Settings.Default.WDriveLetter = WDrive.Drive;
            Settings.Default.WDriveDomain = (int)WDrive.Domain;

            Settings.Default.DSICDriveLetter = DSICDrive.Drive;
            Settings.Default.DSICDrivePassword = SavePasswords ? DSICDrive.Password : null;

            Settings.Default.Save();
        }

        public void ClearSettings()
        {
            Settings.Default.Reset();
        }

        public async Task Shutdown()
        {
            Task dd, dw;

            if (DSICDrive.Connected) dd = DSICDrive.DisconnectAsync();
            else dd = Task.CompletedTask;

            if (WDrive.Connected) dw = WDrive.DisconnectAsync();
            else dw = Task.CompletedTask;

            await Task.WhenAll(dd, dw);

            if (VPN_DSIC.Connected) await VPN_DSIC.DisconnectAsync();
            if (VPN_UPV.Connected) await VPN_UPV.DisconnectAsync();
        }
    }
}
