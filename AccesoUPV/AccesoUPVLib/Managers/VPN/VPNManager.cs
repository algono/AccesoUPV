using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public class VPNManager : ConnectionManager
    {
        //public static int TEST_PING_TIMEOUT = 4000;
        public string ConnectedName { get; private set; }
        public string Name { get; set; }
        public string Server { get; }
        public string TestServer { get; }
        public override bool Connected
        {
            get { return ConnectedName != null; }
            protected set
            {
                if (value) ConnectedName = Name;
                else ConnectedName = null;
            }
        }

        protected ProcessStartInfo pingInfo;
        protected IDictionary creationParams;

        public VPNManager(string server, string name = null, string testServer = null, IDictionary creationParameters = null) : base()
        {
            Name = name;
            Server = server;
            TestServer = testServer ?? server;
            creationParams = creationParameters;

            conInfo.FileName = "rasphone.exe";
            disInfo.FileName = "rasdial.exe";

            pingInfo = new ProcessStartInfo("ping.exe");
            pingInfo.CreateNoWindow = true;
            pingInfo.UseShellExecute = false;
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

        protected override void ConnectionHandler(string output, string err)
        {
            throw new NotImplementedException();
        }

        protected override Process DisconnectProcess()
        {
            disInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            return Process.Start(disInfo);
        }

        protected override void DisconnectionHandler(string output, string err)
        {
            throw new NotImplementedException();
        }

        protected PowerShell CreateShell()
        {
            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", Name);
            shell.AddParameter("ServerAddress", Server);
            shell.AddParameter("RememberCredential"); //Se asegura de que las credenciales se guarden cuando toca (si el usuario lo indica en rasphone)
            if (creationParams != null) shell.AddParameters(creationParams);
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
