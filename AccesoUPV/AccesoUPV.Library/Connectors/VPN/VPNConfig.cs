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
        public string Website { get; set; }
        public IConnectionTestMethod Test { get; set; }
        public IDictionary CreationParameters { get; set; }

        public VPNConfig(string server, string website = null, IConnectionTestMethod test = null, IDictionary creationParameters = null)
        {
            Server = server;
            Website = website;
            Test = test;
            CreationParameters = creationParameters;
        }

        public void Open()
        {
            if (Website == null) throw new InvalidOperationException($"La propiedad {nameof(Website)} es null");
            Process.Start("http://" + Website);
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
        public IEnumerable<PSObject> Find() => Find(Server);

        public bool Any() => Any(Server);

        public IEnumerable<string> FindNames() => FindNames(Server);

        public async Task<IEnumerable<PSObject>> FindAsync() => await FindAsync(Server);

        public async Task<bool> AnyAsync() => await AnyAsync(Server);

        public async Task<IEnumerable<string>> FindNamesAsync() => await FindNamesAsync(Server);
        #endregion

        #region Static
        public static bool Any(string server) => Find(server).Any();

        public static IEnumerable<string> FindNames(string server)
            => Find(server).Select(vpn => vpn.GetName());

        public static IEnumerable<PSObject> Find(string server)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript(GetFindScript(server));
                return shell.Invoke().Where(item => item != null);
            }
        }

        public static async Task<bool> AnyAsync(string server) => (await FindAsync(server)).Any();

        public static async Task<IEnumerable<string>> FindNamesAsync(string server)
            => (await FindAsync(server)).Select(vpn => vpn.GetName());

        public static Task<IEnumerable<PSObject>> FindAsync(string server)
        {
            PowerShell shell = PowerShell.Create();
            shell.AddScript(GetFindScript(server));

            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) =>
            {
                IEnumerable<PSObject> psOutput = shell.EndInvoke(res);
                shell.Dispose();
                return psOutput.Where(item => item != null);
            });
        }

        private static string GetFindScript(string server)
            => "Get-VpnConnection | Where-Object {$_.ServerAddress -eq '" + server + "'}";
        #endregion
        #endregion
    }
}