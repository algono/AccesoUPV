using AccesoUPV.Library.Managers;
using AccesoUPV.Library.Managers.Drive;
using AccesoUPV.Library.Managers.VPN;
using AccesoUPV.Library.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Services
{
    public class AccesoUPVService
    {
        //Managers
        public VPNToUPV VPN_UPV { get; }
        public VPNToDSIC VPN_DSIC { get; }
        public NetworkDriveW WDrive { get; }
        public NetworkDriveDSIC DSICDrive { get; }

        private string user;

        public string User
        {
            get { return user; }
            set
            {
                user = value;
                WDrive.UserName = value;
                DSICDrive.UserName = value;
            }
        }
        public bool SavePasswords { get; set; }

        public bool AreSettingsUninitialized { get { return UninitializedSettings.Count > 0; } }
        public List<SettingsPropertyValue> UninitializedSettings { get; } = new List<SettingsPropertyValue>();

        public AccesoUPVService()
        {
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;

            user = Settings.Default.User;

            VPN_UPV = new VPNToUPV(Settings.Default.VPN_UPVName);
            VPN_DSIC = new VPNToDSIC(Settings.Default.VPN_DSICName);

            WDrive = new NetworkDriveW(User, Settings.Default.WDriveLetter, (UPVDomain) Settings.Default.WDriveDomain);

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
            Settings.Default.WDriveDomain = (int) WDrive.Domain;

            Settings.Default.DSICDriveLetter = DSICDrive.Drive;
            Settings.Default.DSICDrivePassword = SavePasswords ? DSICDrive.Password : null;

            Settings.Default.Save();
        }

        public static void ConnectToLinuxDesktop() => Utilities.ConnectToRemoteDesktop(Servers.LINUX_DSIC);
        public static void ConnectToWindowsDesktop() => Utilities.ConnectToRemoteDesktop(Servers.WIN_DSIC);
    }
}
