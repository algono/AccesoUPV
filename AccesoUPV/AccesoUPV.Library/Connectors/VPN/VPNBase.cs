using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public abstract class VPNBase : ProcessConnector, IVPN
    {
        public const int CONNECTED_PING_TIMEOUT = 5000, DISCONNECTED_PING_TIMEOUT = 500;

        public string ConnectedName { get; private set; }
        public string Name { get; set; }
        public abstract string Server { get; }
        public abstract string TestServer { get; }
        public override bool Connected
        {
            get => ConnectedName != null;
            protected set => ConnectedName = value ? Name : null;
        }

        protected static readonly ProcessStartInfo PingInfo = CreateProcessInfo("ping.exe");

        protected VPNBase(string name = null)
        {
            Name = name;

            conInfo.FileName = "rasphone.exe";
            disInfo.FileName = "rasdial.exe";
        }

        public bool IsReachable() => IsReachable(Connected ? CONNECTED_PING_TIMEOUT : DISCONNECTED_PING_TIMEOUT);

        public bool IsReachable(int timeout)
        {
            if (string.IsNullOrEmpty(TestServer)) throw new ArgumentNullException("The test server is not defined.");
            PingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            Process p = Process.Start(PingInfo);
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        public void CheckConnection()
        {
            Connected = IsActuallyConnected();
        }

        private bool IsActuallyConnected()
        {
            bool res = false;
            Process checkingProcess = Process.Start(CreateProcessInfo("rasdial.exe"));
            checkingProcess.WaitAndCheck((s, o, e) =>
            {
                res = s && o.Contains(Name);
            });
            return res;
        }

        protected override Process ConnectProcess()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("The name is not defined.");
            conInfo.Arguments = $"-d \"{Name}\"";
            return Process.Start(conInfo);
        }
        /**
         * @throws:
         * - ArgumentException: La VPN que se ha proporcionado funciona, pero es incapaz de acceder al Test Server
         * - OperationCanceledException: El usuario canceló la operación.
         */
        protected override void ConnectionHandler(bool succeeded, string output, string error)
        {
            if (succeeded)
            {
                try
                {
                    if (!IsActuallyConnected()) throw new OperationCanceledException();

                    if (!IsReachable(CONNECTED_PING_TIMEOUT))
                    {
                        disInfo.Arguments = $"\"{Name}\" /DISCONNECT";
                        Process.Start(disInfo).WaitAndCheck();
                        throw new ArgumentException(); // VPN no puede acceder al TestServer
                    }
                }
                catch (IOException)
                {
                    //If the checking fails, it still continues
                }
            }

            base.ConnectionHandler(succeeded, output, error);

        }

        protected override Process DisconnectProcess()
        {
            disInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            return Process.Start(disInfo);
        }

        protected virtual PowerShell CreateShell()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("The name is not defined.");
            else if (string.IsNullOrEmpty(Server)) throw new ArgumentNullException("The server is not defined.");

            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", Name);
            shell.AddParameter("ServerAddress", Server);
            //Es necesario para que las credenciales se guarden cuando el usuario lo indique en rasphone
            shell.AddParameter("RememberCredential");
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

        public bool SetNameAuto()
        {
            List<PSObject> vpnList = Find();
            if (vpnList.Count > 0)
            {
                Name = vpnList[0].GetStringPropertyValue("Name");
                return true;
            }

            return false;
        }

        public bool Exists() => Find().Exists(vpn => vpn.GetStringPropertyValue("Name") == Name);

        public List<PSObject> Find() => Find(Server);

        public bool Any() => Any(Server);

        public static bool Any(string server) => Find(server).Count > 0;

        public List<string> FindNames() => FindNames(Server);

        public static List<string> FindNames(string server)
            => Find(server).Select(vpn => vpn.GetStringPropertyValue("Name")).ToList();

        public static List<PSObject> Find(string server)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript(GetFindScript(server));
                List<PSObject> PSOutput = shell.Invoke().ToList();
                PSOutput.RemoveAll(item => item == null);
                return PSOutput;
            }
        }

        public async Task<bool> SetNameAutoAsync()
        {
            List<PSObject> vpnList = await FindAsync();
            if (vpnList.Count > 0)
            {
                Name = vpnList[0].GetStringPropertyValue("Name");
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync() => (await FindAsync()).Exists(vpn => vpn.GetStringPropertyValue("Name") == Name);

        public async Task<List<PSObject>> FindAsync() => await FindAsync(Server);

        public async Task<bool> AnyAsync() => await AnyAsync(Server);

        public static async Task<bool> AnyAsync(string server) => (await FindAsync(server)).Count > 0;

        public async Task<List<string>> FindNamesAsync() => await FindNamesAsync(Server);

        public static async Task<List<string>> FindNamesAsync(string server)
            => (await FindAsync(server)).Select(vpn => vpn.GetStringPropertyValue("Name")).ToList();

        public static Task<List<PSObject>> FindAsync(string server)
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript(GetFindScript(server));

            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                List<PSObject> PSOutput = shell.EndInvoke(res).ToList();
                shell.Dispose();
                PSOutput.RemoveAll(item => item == null);
                return PSOutput;
            });
        }

        private static string GetFindScript(string server)
            => "Get-VpnConnection | Where-Object {$_.ServerAddress -eq '" + server + "'}";
    }
}
