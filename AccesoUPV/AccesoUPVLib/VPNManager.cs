using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public abstract class VPNManager : ConnectionManager<bool>
    {
        public VPN ConnectedVPN { get; protected set; }
        public VPN ManagedVPN { get; set; }
        public bool Connected { get { return ConnectedVPN != null; } }
        
        protected Process proc;
        protected ProcessStartInfo conInfo, disInfo;

        public VPNManager()
        { 
            proc = new Process();

            conInfo = new ProcessStartInfo("rasphone.exe");
            conInfo.CreateNoWindow = true;

            disInfo = new ProcessStartInfo("rasdial.exe");
            disInfo.CreateNoWindow = true;
        }

        public Task ExitedAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            proc.Exited += (s, e) => tcs.TrySetResult(null);
            if (proc.HasExited) tcs.TrySetResult(null);
            return tcs.Task;
        }

        public async Task<bool> connect()
        {
            conInfo.Arguments = $"-d \"{ManagedVPN.Name}\"";
            proc.StartInfo = conInfo;
            proc.Start();
            await ExitedAsync();
            if (proc.ExitCode == 0) ConnectedVPN = ManagedVPN;
            return proc.ExitCode == 0;
        }

        public async Task<bool> disconnect()
        {
            disInfo.Arguments = $"\"{ConnectedVPN.Name}\" /DISCONNECT";
            proc.StartInfo = disInfo;
            proc.Start();
            await ExitedAsync();
            return proc.ExitCode == 0;
        }
    }
}
