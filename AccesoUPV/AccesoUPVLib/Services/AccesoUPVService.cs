using AccesoUPV.Lib.Managers;
using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using AccesoUPV.Lib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace AccesoUPV.Lib.Services
{
    public class AccesoUPVService
    {
        //Managers
        public UPVManager VPN_UPV { get; }
        public DSICManager VPN_DSIC { get; }
        public WDriveManager WDrive { get; }
        public DSICDriveManager DSICDrive { get; }

        private string user;

        public string User
        {
            get { return user; }
            set
            {
                user = value;
                WDrive.User = value;
                DSICDrive.User = value;
            }
        }
        public bool SavePasswords { get; set; }

        public bool AreSettingsUninitialized { get { return UninitializedSettings.Count > 0; } }
        public HashSet<SettingsPropertyValue> UninitializedSettings { get; } = new HashSet<SettingsPropertyValue>();

        public AccesoUPVService()
        {
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;

            user = Settings.Default.User;

            VPN_UPV = new UPVManager(Settings.Default.VPN_UPVName);
            VPN_DSIC = new DSICManager(Settings.Default.VPN_DSICName);

            WDrive = new WDriveManager(User, Settings.Default.WDriveLetter);

            //En las strings de settings, si no estan inicializados, son strings vacios
            if (!string.IsNullOrEmpty(Settings.Default.WDriveDomain)) WDrive.Domain = Settings.Default.WDriveDomain;
            
            DSICDrive = new DSICDriveManager(User, Settings.Default.DSICDrivePassword, Settings.Default.DSICDriveLetter);
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
            Settings.Default.WDriveDomain = WDrive.Domain;

            Settings.Default.DSICDriveLetter = DSICDrive.Drive;
            Settings.Default.DSICDrivePassword = SavePasswords ? DSICDrive.Password : null;

            Settings.Default.Save();
        }

        public static void ConnectToLinuxDesktop()
        {
            ConnectToRemoteDesktop(Servers.LINUX_DSIC);
        }
        public static void ConnectToWindowsDesktop()
        {
            ConnectToRemoteDesktop(Servers.WIN_DSIC);
        }
        protected static void ConnectToRemoteDesktop(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

    }
}
