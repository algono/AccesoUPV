using AccesoUPV.Library.Static;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public partial class VPN
    {
        #region Properties
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

        private static readonly ProcessStartInfo
            ConnectionInfo = CreateProcessInfo("rasphone.exe"),
            DisconnectionInfo = CreateProcessInfo("rasdial.exe");
        #endregion

        #region Connection methods
        protected override string GetConnectProcessArguments() => $"-d \"{Name}\"";
        protected override string GetDisconnectProcessArguments() => $"\"{ConnectedName}\" /DISCONNECT";
        #endregion

        #region Utility methods

        private static string GetName(PSObject obj) => obj.GetName();

        private static List<PSObject> GetList()
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript("Get-VpnConnection");
                List<PSObject> psOutput = shell.Invoke().ToList();
                psOutput.RemoveAll(item => item == null);
                return psOutput;
            }
        }

        private static Task<List<PSObject>> GetListAsync()
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
