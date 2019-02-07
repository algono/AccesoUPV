using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public abstract class VPNManagerBase : ConnectionManager
    {
        public const int TEST_PING_TIMEOUT = 4000;
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
        public bool IsReachable(int timeout = 500)
        {
            pingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            Process p = Process.Start(pingInfo);
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        protected override Process ConnectProcess()
        {
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
                succeeded = checkingProcess.WaitAndCheck((s, o, e) => 
                {
                    if (s && !o.Contains(Name)) throw new OperationCanceledException();
                });
                if (succeeded && !IsReachable(TEST_PING_TIMEOUT))
                {
                    disInfo.Arguments = $"\"{Name}\" /DISCONNECT";
                    Process.Start(disInfo).WaitAndCheck();
                    throw new ArgumentException(); //VPN no valida para acceder al TestServer
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
            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", Name);
            shell.AddParameter("ServerAddress", Server);
            shell.AddParameter("RememberCredential"); //Se asegura de que las credenciales se guarden cuando toca (si el usuario lo indica en rasphone)
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
