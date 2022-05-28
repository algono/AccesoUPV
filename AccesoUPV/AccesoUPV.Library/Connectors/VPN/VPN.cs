using AccesoUPV.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPN : ProcessConnector, IOpenable
    {
        public const int ConnectedPingTimeout = 5000, DisconnectedPingTimeout = 500;

        #region Properties
        public string ConnectedName { get; private set; }
        public string Name { get; set; }

        public override bool IsConnected
        {
            get => ConnectedName != null;
            protected set => ConnectedName = value ? Name : null;
        }

        public bool IsActuallyConnected
        {
            get
            {
                bool res = false;
                Process checkingProcess = Process.Start(CreateProcessInfo("rasdial.exe"));
                checkingProcess.WaitAndCheck((args) =>
                {
                    res = args.Succeeded && args.Output.Contains(Name);
                });
                return res;
            }
        }

        public VPNConfig Config { get; }

        private static readonly ProcessStartInfo
            ConnectionInfo = CreateProcessInfo("rasphone.exe"),
            DisconnectionInfo = CreateProcessInfo("rasdial.exe");
        #endregion

        public VPN(string server, string name = null) : this(new VPNConfig(server), name) { }

        public VPN(VPNConfig config, string name = null)
        {
            Config = config;
            Name = name;
        }

        #region Connection methods
        protected override Process ConnectProcess()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
            ConnectionInfo.Arguments = $"-d \"{Name}\"";
            return Process.Start(ConnectionInfo);
        }
        /**
         * @throws:
         * - ArgumentException: La VPN que se ha proporcionado funciona, pero es incapaz de acceder al Test Server
         * - OperationCanceledException: El usuario canceló la operación.
         */
        protected override void OnProcessConnected(ProcessEventArgs e)
        {
            if (e.Succeeded)
            {
                try
                {
                    if (!IsActuallyConnected) throw new OperationCanceledException();

                    base.OnProcessConnected(e);

                    if (!IsReachable())
                    {
                        Disconnect();
                        throw new ArgumentException(Config.Test.GetErrorMessage("UPV"));
                    }
                }
                catch (IOException)
                {
                    // If the checking fails, it still continues
                }
            }
            else
            {
                base.OnProcessConnected(e);
            }

        }

        protected override Process DisconnectProcess()
        {
            DisconnectionInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            return Process.Start(DisconnectionInfo);
        }
        #endregion

        #region Creation methods
        public bool Create() => Config.Create(Name);
        public Task<bool> CreateAsync() => Config.CreateAsync(Name);
        #endregion

        public void Open() => Config.Open();

        #region Utility methods
        public bool IsReachable() => Config.Test.IsReachable(IsConnected ? ConnectedPingTimeout : DisconnectedPingTimeout);

        public void UpdateConnectionStatus() => IsConnected = IsActuallyConnected;

        public bool SetNameAuto()
        {
            List<PSObject> vpnList = Config.Find();
            if (vpnList.Count <= 0) return false;
            Name = vpnList[0].GetName();
            return true;

        }

        public bool Exists() => Config.Find().Exists(vpn => vpn.GetName() == Name);

        public static List<string> GetNameList() => GetList().Select(vpn => vpn.GetName()).ToList();

        public static List<PSObject> GetList()
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript("Get-VpnConnection");
                List<PSObject> psOutput = shell.Invoke().ToList();
                psOutput.RemoveAll(item => item == null);
                return psOutput;
            }
        }

        public async Task<bool> SetNameAutoAsync()
        {
            List<PSObject> vpnList = await Config.FindAsync();
            if (vpnList.Count <= 0) return false;
            Name = vpnList[0].GetName();
            return true;

        }

        public async Task<bool> ExistsAsync() => (await Config.FindAsync()).Exists(vpn => vpn.GetName() == Name);

        public static async Task<List<string>> GetNameListAsync() => (await GetListAsync()).Select(vpn => vpn.GetName()).ToList();

        public static Task<List<PSObject>> GetListAsync()
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript("Get-VpnConnection");

            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                List<PSObject> psOutput = shell.EndInvoke(res).ToList();
                shell.Dispose();
                psOutput.RemoveAll(item => item == null);
                return psOutput;
            });
        }
        #endregion
    }
}
