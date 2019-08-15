using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Properties;
using System.Reflection;

namespace AccesoUPV.Library.Services
{
    public class AccesoUPVService : IAccesoUPVService
    {
        #region Connectables
        public VPN VPN_UPV { get; }
        public VPN VPN_DSIC { get; }
        public NetworkDrive<UPVDomain> Disco_W { get; }
        public NetworkDrive Disco_DSIC { get; }
        #endregion

        #region Preferences
        private string _user;

        public string User
        {
            get => _user;
            set
            {
                _user = value;
                Disco_W.Username = value;
                Disco_DSIC.Username = value;
            }
        }
        public bool SavePasswords { get; set; }

        #endregion

        #region Settings Properties
        public bool AreUninitializedSettings => UninitializedSettings.Count > 0;
        public List<SettingsPropertyValue> UninitializedSettings { get; } = new List<SettingsPropertyValue>();
        #endregion

        #region Connectables Reflection
        private IEnumerable<Connectable> Connectables => connectablesInfo.GetValues<Connectable>(this);

        private static readonly IEnumerable<PropertyInfo> connectablesInfo = typeof(AccesoUPVService).GetProperties().AsEnumerable().WherePropertiesAreOfType<Connectable>();
        #endregion

        public event EventHandler<ShutdownEventArgs> ShuttingDown;

        public AccesoUPVService()
        {
            Settings.Default.SettingsLoaded += Default_SettingsLoaded;

            _user = Settings.Default.User;

            VPN_UPV = VPNFactory.GetVPNToUPV(Settings.Default.VPN_UPVName);
            VPN_DSIC = VPNFactory.GetVPNToDSIC(Settings.Default.VPN_DSICName);

            Disco_W = DriveFactory.GetDriveW(Settings.Default.WDriveLetter, User, (UPVDomain) Settings.Default.WDriveDomain);

            Disco_DSIC = DriveFactory.GetDriveDSIC(Settings.Default.DSICDriveLetter, User, Settings.Default.DSICDrivePassword);
            SavePasswords = !string.IsNullOrEmpty(Disco_DSIC.Password);
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

            Settings.Default.WDriveLetter = Disco_W.Drive;
            Settings.Default.WDriveDomain = (int)Disco_W.Domain;

            Settings.Default.DSICDriveLetter = Disco_DSIC.Drive;
            Settings.Default.DSICDrivePassword = SavePasswords ? Disco_DSIC.Password : null;

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
            foreach (Connectable connectable in Connectables)
            {
                if (connectable.Connected) connectable.Disconnect();
            }
        }

        public async Task ShutdownAsync(IProgress<string> progress = null)
        {
            List<Func<Task>> driveTasks = new List<Func<Task>>();
            List<Func<Task>> VPNTasks = new List<Func<Task>>();

            int steps = 0;

            foreach (PropertyInfo info in connectablesInfo)
            {
                Connectable connectable = info.GetValue(this) as Connectable;
                if (connectable.Connected)
                {
                    steps++;
                    string nameToDisplay = info.Name.Replace('_', ' ');
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

        private async Task ShutdownConnectable(Connectable connectable, string name, IProgress<string> progress = null)
        {
            progress?.Report($"Desconectando { name }");
            await connectable.DisconnectAsync();
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
