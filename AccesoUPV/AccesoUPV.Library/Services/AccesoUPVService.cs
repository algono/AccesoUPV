﻿using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Interfaces;
using AccesoUPV.Library.Properties;
using AccesoUPV.Library.Static;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Services
{
    public class AccesoUPVService : IAccesoUPVService
    {
        #region Connectables
        public VPN VPN_UPV { get; }
        public VPN VPN_DSIC { get; }
        public NetworkDrive<UPVDomain> Disco_W { get; }
        public NetworkDrive Disco_DSIC { get; }
        public NetworkDrive Asig_DSIC { get; }
        #endregion

        #region Preferences
        private string _user;

        public string User
        {
            get => _user;
            set
            {
                // Dont allow empty strings
                if (value?.Length == 0) value = null;

                _user = value;

                foreach (NetworkDrive drive in NetworkDrives)
                {
                    if (drive != null)
                    {
                        drive.Username = value;
                    }
                }
            }
        }
        public bool SavePasswords { get; set; }

        public bool NotifyIcon { get; set; }

        #endregion

        #region Settings Properties
        public bool AreUninitializedSettings => UninitializedSettings.Count > 0;
        public List<SettingsPropertyValue> UninitializedSettings { get; } = new List<SettingsPropertyValue>();
        #endregion

        #region Connectables Reflection
        private IEnumerable<IConnectable> Connectables => connectablesInfo.GetValues<IConnectable>(this);
        private IEnumerable<NetworkDrive> NetworkDrives => connectablesInfo.GetValuesOfType<NetworkDrive>(this);

        private static readonly IEnumerable<PropertyInfo> connectablesInfo = typeof(AccesoUPVService).GetProperties().AsEnumerable().WherePropertiesAreOfType<IConnectable>();
        #endregion

        private static readonly IReadOnlyList<string> UPVWiFiNetworks = new List<string> { "UPVNET", "eduroam", "UPV-IoT" };

        public event EventHandler<ShutdownEventArgs> ShuttingDown;

        public AccesoUPVService()
        {
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;

            User = Settings.Default.User;

            VPN_UPV = VPNFactory.GetVPNToUPV(Settings.Default.VPN_UPVName);
            VPN_DSIC = VPNFactory.GetVPNToDSIC(Settings.Default.VPN_DSICName);

            char wDriveLetter = DriveLetterTools.ValidOrDefault(Settings.Default.WDriveLetter);

            Disco_W = DriveFactory.GetDriveW(wDriveLetter, User, (UPVDomain)Settings.Default.WDriveDomain);

            string DSICDrivePassword = Settings.Default.DSICDrivePassword;
            if (!string.IsNullOrEmpty(DSICDrivePassword)) DSICDrivePassword = PasswordHelper.Decrypt(DSICDrivePassword);

            char AsigDSICDriveLetter = DriveLetterTools.ValidOrDefault(Settings.Default.AsigDSICDriveLetter);

            Asig_DSIC = DriveFactory.GetAsigDriveDSIC(AsigDSICDriveLetter, User, DSICDrivePassword);

            char DSICDriveLetter = DriveLetterTools.ValidOrDefault(Settings.Default.DSICDriveLetter);

            Disco_DSIC = DriveFactory.GetDriveDSIC(DSICDriveLetter, User, DSICDrivePassword);
            SavePasswords = !string.IsNullOrEmpty(Disco_DSIC.Password);

            NotifyIcon = Settings.Default.NotifyIcon;
        }

        #region Settings methods
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

            Settings.Default.WDriveLetter = Disco_W.Letter;
            Settings.Default.WDriveDomain = (int)Disco_W.Domain;

            Settings.Default.DSICDriveLetter = Disco_DSIC.Letter;
            Settings.Default.DSICDrivePassword = SavePasswords ? PasswordHelper.Encrypt(Disco_DSIC.Password) : null;

            Settings.Default.NotifyIcon = NotifyIcon;

            Settings.Default.Save();
        }

        public void ClearSettings()
        {
            Settings.Default.Reset();
        }
        #endregion

        #region Shutdown
        public void Shutdown()
        {
            foreach (IConnectable connectable in Connectables)
            {
                if (connectable.IsConnected) connectable.Disconnect();
            }
        }

        public async Task ShutdownAsync(IProgress<string> progress = null)
        {
            List<Func<Task>> driveTasks = new List<Func<Task>>();
            List<Func<Task>> VPNTasks = new List<Func<Task>>();

            int steps = 0;

            foreach (PropertyInfo info in connectablesInfo)
            {
                IConnectable connectable = info.GetValue(this) as IConnectable;
                if (connectable.IsConnected)
                {
                    steps++;
                    string nameToDisplay = connectable is INameable nameable ? nameable.Name : info.Name.Replace('_', ' ');
                    Task shutdownTask() => ShutdownConnectable(connectable, nameToDisplay, progress);

                    switch (connectable)
                    {
                        case NetworkDrive _:
                            driveTasks.Add(shutdownTask);
                            break;
                        case VPN _:
                            VPNTasks.Add(shutdownTask);
                            break;
                    }
                }
            }

            OnShuttingDown(this, new ShutdownEventArgs(steps));

            if (driveTasks.Count > 0) await Task.WhenAll(driveTasks.Select(func => func()));

            foreach (Func<Task> disconnectVPN in VPNTasks) await disconnectVPN();

            progress?.Report("Saliendo");
        }

        protected virtual void OnShuttingDown(object sender, ShutdownEventArgs e)
        {
            ShuttingDown?.Invoke(sender, e);
        }

        private async Task ShutdownConnectable(IConnectable connectable, string name, IProgress<string> progress = null)
        {
            progress?.Report($"Desconectando { name }");
            await connectable.DisconnectAsync();
        }

        #endregion

        #region Reset Connection
        public bool ResetUPVConnection()
        {
            if (VPN_UPV.IsConnected)
            {
                VPN_UPV.Reconnect();
            }
            else
            {
                return ResetUPVWifiConnection();
            }

            return true;
        }

        public async Task<bool> ResetUPVConnectionAsync()
        {
            if (VPN_UPV.IsConnected)
            {
                await VPN_UPV.ReconnectAsync();
            }
            else
            {
                return await ResetUPVWifiConnectionAsync();
            }

            return true;
        }

        private const int ConnectedWiFiTimeout = 5000;
        private const string ResetTimeoutMessage =
            "La UPV todavía no está disponible. Espere unos segundos, y vuelva a intentarlo.\n\n"
            + "Si sigue sin ser capaz de conectarse, reconecte la red WiFi manualmente.";

        protected bool ResetUPVWifiConnection()
        {
            string connectedWiFi = Utilities.IsAnyWiFiConnectionUp(UPVWiFiNetworks);
            bool isAnyWiFiConnectionUp = !string.IsNullOrEmpty(connectedWiFi);

            if (isAnyWiFiConnectionUp)
            {
                Utilities.ResetWiFiConnection(connectedWiFi);
                bool reachable = VPN_UPV.Config.WaitUntilReachable(ConnectedWiFiTimeout);
                if (!reachable) throw new TimeoutException(ResetTimeoutMessage);
            }

            return isAnyWiFiConnectionUp;
        }

        protected async Task<bool> ResetUPVWifiConnectionAsync()
        {
            string connectedWiFi = await Utilities.IsAnyWiFiConnectionUpAsync(UPVWiFiNetworks);
            bool isAnyWiFiConnectionUp = !string.IsNullOrEmpty(connectedWiFi);

            if (isAnyWiFiConnectionUp)
            {
                await Utilities.ResetWiFiConnectionAsync(connectedWiFi);
                bool reachable = await VPN_UPV.Config.WaitUntilReachableAsync(ConnectedWiFiTimeout);
                if (!reachable) throw new TimeoutException(ResetTimeoutMessage);
            }

            return isAnyWiFiConnectionUp;
        }
        #endregion

    }

    public class ShutdownEventArgs : EventArgs
    {
        public int Steps { get; }

        public ShutdownEventArgs(int steps)
        {
            Steps = steps;
        }
    }
}
