using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public abstract class VPNManager : ConnectionManager<bool>
    {
        public static int TEST_PING_TIMEOUT = 4000;
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

        public VPNManager(string name, string server, string testServer) : base()
        {
            Name = name;
            Server = server;
            TestServer = testServer;

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

        public override bool Connect()
        {
            conInfo.Arguments = $"-d \"{Name}\"";
            Process proc = Process.Start(conInfo);
            CheckProcess(proc);
            return Connected;
        }

        public override bool Disconnect()
        {
            disInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            Process proc = Process.Start(disInfo);
            CheckProcess(proc);
            return Connected;
        }

        public override Task<bool> ConnectAsync()
        {
            conInfo.Arguments = $"-d \"{Name}\"";
            Process proc = Process.Start(conInfo);

            return CheckProcessAsync(proc);
        }

        public override Task<bool> DisconnectAsync()
        {
            disInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            Process proc = Process.Start(disInfo);

            return CheckProcessAsync(proc);
        }

        protected abstract PowerShell CreateShell();

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
    }
}
