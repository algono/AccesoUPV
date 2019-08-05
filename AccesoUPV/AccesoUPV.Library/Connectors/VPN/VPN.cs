using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPN : ProcessConnector, Openable
    {
        public const int ConnectedPingTimeout = 5000, DisconnectedPingTimeout = 500;

        public string ConnectedName { get; private set; }
        public string Name { get; set; }

        public override bool Connected
        {
            get => ConnectedName != null;
            protected set => ConnectedName = value ? Name : null;
        }

        public VPNConfig Config { get; }

        private static readonly ProcessStartInfo ConnectionInfo, DisconnectionInfo;

        static VPN()
        {
            ConnectionInfo = CreateProcessInfo("rasphone.exe");
            DisconnectionInfo = CreateProcessInfo("rasdial.exe");
        }

        public VPN(string server, string name = null) : this(new VPNConfig(server), name)
        {
        }

        public VPN(VPNConfig config, string name = null)
        {
            Config = config;
            Name = name;
        }

        public bool IsReachable() => Config.IsReachable(Connected ? ConnectedPingTimeout : DisconnectedPingTimeout);

        public void CheckConnection()
        {
            Connected = IsActuallyConnected();
        }

        private bool IsActuallyConnected()
        {
            bool res = false;
            Process checkingProcess = Process.Start(CreateProcessInfo("rasdial.exe"));
            checkingProcess.WaitAndCheck((args) =>
            {
                res = args.Succeeded && args.Output.Contains(Name);
            });
            return res;
        }

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
                    if (!IsActuallyConnected()) throw new OperationCanceledException();

                    base.OnProcessConnected(e);

                    if (!IsReachable())
                    {
                        Disconnect();
                        throw new ArgumentException($"La VPN no puede acceder al servidor: {Config.TestServer}");
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

        protected virtual PowerShell CreateShell()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException(nameof(Name));
            if (string.IsNullOrEmpty(Config.Server)) throw new ArgumentNullException(nameof(Config.Server));

            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", Name);
            shell.AddParameter("ServerAddress", Config.Server);

            //Es necesario para que las credenciales se guarden cuando el usuario lo indique en rasphone
            shell.AddParameter("RememberCredential");

            shell.AddParameters(Config.CreationParameters);

            return shell;
        }

        public bool Create()
        {
            using (PowerShell shell = CreateShell())
            {
                shell.Invoke();
                return !shell.HadErrors;
            }
        }
        public Task<bool> CreateAsync()
        {
            PowerShell shell = CreateShell();
            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                shell.EndInvoke(res);
                bool succeeded = shell.HadErrors;
                shell.Dispose();
                return succeeded;
            });
        }

        public void Open() => Config.Open();

        public bool SetNameAuto()
        {
            List<PSObject> vpnList = Find();
            if (vpnList.Count <= 0) return false;
            Name = vpnList[0].GetStringPropertyValue("Name");
            return true;

        }

        public bool Exists() => Find().Exists(vpn => vpn.GetStringPropertyValue("Name") == Name);

        public List<PSObject> Find() => VPNConfig.Find(Config.Server);

        public bool Any() => VPNConfig.Any(Config.Server);

        public List<string> FindNames() => VPNConfig.FindNames(Config.Server);

        public static List<string> GetNameList() => GetList().Select(vpn => vpn.GetStringPropertyValue("Name")).ToList();

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
            List<PSObject> vpnList = await FindAsync();
            if (vpnList.Count <= 0) return false;
            Name = vpnList[0].GetStringPropertyValue("Name");
            return true;

        }

        public async Task<bool> ExistsAsync() => (await FindAsync()).Exists(vpn => vpn.GetStringPropertyValue("Name") == Name);

        public async Task<List<PSObject>> FindAsync() => await VPNConfig.FindAsync(Config.Server);

        public async Task<bool> AnyAsync() => await VPNConfig.AnyAsync(Config.Server);

        public async Task<List<string>> FindNamesAsync() => await VPNConfig.FindNamesAsync(Config.Server);

        public static async Task<List<string>> GetNameListAsync() => (await GetListAsync()).Select(vpn => vpn.GetStringPropertyValue("Name")).ToList();

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
    }
}
