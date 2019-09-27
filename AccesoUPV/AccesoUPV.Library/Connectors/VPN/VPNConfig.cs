using AccesoUPV.Library.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPNConfig : IOpenable
    {
        public string Server { get; set; }
        public string TestServer { get; set; }
        public IDictionary CreationParameters { get; set; }

        public VPNConfig(string server, string testServer = null, IDictionary creationParameters = null)
        {
            Server = server;
            TestServer = testServer;
            CreationParameters = creationParameters;
        }

        public void Open()
        {
            Process.Start("http://" + TestServer);
        }

        public bool IsReachable(int timeout)
        {
            if (string.IsNullOrEmpty(TestServer)) throw new ArgumentNullException(nameof(TestServer));

            ProcessStartInfo pingInfo = ProcessConnector.CreateProcessInfo("ping.exe");
            pingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            Process p = Process.Start(pingInfo);
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        #region Creation methods
        protected virtual PowerShell CreateShell(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(Server)) throw new ArgumentNullException(nameof(Server));

            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", name);
            shell.AddParameter("ServerAddress", Server);

            //Es necesario para que las credenciales se guarden cuando el usuario lo indique en rasphone
            shell.AddParameter("RememberCredential");

            shell.AddParameters(CreationParameters);

            return shell;
        }

        public bool Create(string name)
        {
            using (PowerShell shell = CreateShell(name))
            {
                shell.Invoke();
                return !shell.HadErrors;
            }
        }
        public Task<bool> CreateAsync(string name)
        {
            PowerShell shell = CreateShell(name);
            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                shell.EndInvoke(res);
                bool succeeded = !shell.HadErrors;
                shell.Dispose();
                return succeeded;
            });
        }
        #endregion

        #region PS Selection methods
        #region Non-static
        public List<PSObject> Find() => Find(Server);

        public bool Any() => Any(Server);

        public List<string> FindNames() => FindNames(Server);

        public async Task<List<PSObject>> FindAsync() => await FindAsync(Server);

        public async Task<bool> AnyAsync() => await AnyAsync(Server);

        public async Task<List<string>> FindNamesAsync() => await FindNamesAsync(Server);
        #endregion

        #region Static
        public static bool Any(string server) => Find(server).Count > 0;

        public static List<string> FindNames(string server)
            => Enumerable.Select(Find(server), vpn => vpn.GetName()).ToList();

        public static List<PSObject> Find(string server)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript(GetFindScript(server));
                List<PSObject> psOutput = shell.Invoke().ToList();
                psOutput.RemoveAll(item => item == null);
                return psOutput;
            }
        }

        public static async Task<bool> AnyAsync(string server) => (await FindAsync(server)).Count > 0;

        public static async Task<List<string>> FindNamesAsync(string server)
            => Enumerable.Select((await FindAsync(server)), vpn => vpn.GetName()).ToList();

        public static Task<List<PSObject>> FindAsync(string server)
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript(GetFindScript(server));

            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                List<PSObject> psOutput = shell.EndInvoke(res).ToList();
                shell.Dispose();
                psOutput.RemoveAll(item => item == null);
                return psOutput;
            });
        }

        private static string GetFindScript(string server)
            => "Get-VpnConnection | Where-Object {$_.ServerAddress -eq '" + server + "'}";
        #endregion
        #endregion
    }
}