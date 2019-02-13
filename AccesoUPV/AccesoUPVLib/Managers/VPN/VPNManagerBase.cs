using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers.VPN
{
    public abstract class VPNManagerBase : ConnectionManager, IVPNManager
    {
        public const int CONNECTED_PING_TIMEOUT = 5000, DISCONNECTED_PING_TIMEOUT = 500;
        public string ConnectedName { get; private set; }
        public string Name { get; set; }
        public abstract string Server { get; }
        public abstract string TestServer { get; }
        public override bool Connected
        {
            get { return ConnectedName != null; }
            protected set
            {
                if (value) ConnectedName = Name;
                else ConnectedName = null;
            }
        }

        protected readonly ProcessStartInfo pingInfo;

        public VPNManagerBase(string name = null) : base()
        {
            Name = name;

            conInfo.FileName = "rasphone.exe";
            disInfo.FileName = "rasdial.exe";

            pingInfo = CreateProcessInfo("ping.exe");
        }
        public bool IsReachable()
        {
            if (Connected) return IsReachable(CONNECTED_PING_TIMEOUT);
            else return IsReachable(DISCONNECTED_PING_TIMEOUT);
        }
        public bool IsReachable(int timeout)
        {
            if (string.IsNullOrEmpty(TestServer)) throw new ArgumentNullException("The test server is not defined.");
            pingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            Process p = Process.Start(pingInfo);
            p.WaitForExit();
            return p.ExitCode == 0;
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
                Process checkingProcess = Process.Start(CreateProcessInfo("rasdial.exe"));
                try
                {
                    if (!IsActuallyConnected()) throw new OperationCanceledException();
                    //checkingProcess.WaitAndCheck((s, o, e) =>
                    //{
                    //    if (s && !o.Contains(Name)) throw new OperationCanceledException();
                    //});
                    if (!IsReachable(CONNECTED_PING_TIMEOUT))
                    {
                        disInfo.Arguments = $"\"{Name}\" /DISCONNECT";
                        Process.Start(disInfo).WaitAndCheck();
                        throw new ArgumentException(); //VPN no valida para acceder al TestServer
                    }
                }
                catch (IOException)
                {
                    //If the checking fails, it still continues
                }
            }
            
            base.ConnectionHandler(succeeded, output, error);

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
        public Task CreateAsync()
        {
            PowerShell shell = CreateShell();
            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) => shell.Dispose());
        }

        public bool Exists() => Find(Server).Exists(e => ((string) e.Properties["Name"].Value) == Name);
        public List<PSObject> Find() => Find(Server);
        public static List<PSObject> Find(string Server)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript("Get-VpnConnection | Where-Object {$_.ServerAddress -eq '" + Server + "'}");
                List<PSObject> PSOutput = shell.Invoke().ToList();
                PSOutput.RemoveAll(item => item == null);
                return PSOutput;
            }
        }
    }
}
