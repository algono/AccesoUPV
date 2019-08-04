using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class VPNConfig : Openable
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

        public static bool Any(string server) => Find(server).Count > 0;

        public static List<string> FindNames(string server)
            => Enumerable.Select<PSObject, string>(Find(server), vpn => vpn.GetStringPropertyValue("Name")).ToList();

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
            => Enumerable.Select<PSObject, string>((await FindAsync(server)), vpn => vpn.GetStringPropertyValue("Name")).ToList();

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
    }
}