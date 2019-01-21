using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public abstract class ConnectionManager<T>
    {
        public abstract bool Connected { get; protected set; }
        protected ProcessStartInfo conInfo, disInfo;

        public ConnectionManager()
        {
            conInfo = new ProcessStartInfo();
            conInfo.CreateNoWindow = true;
            conInfo.UseShellExecute = false;

            disInfo = new ProcessStartInfo();
            disInfo.CreateNoWindow = true;
            disInfo.UseShellExecute = false;
        }

        protected void CheckProcess(Process proc)
        {
            proc.WaitForExit();
            Connected = proc.ExitCode == 0;
        }

        protected Task<bool> CheckProcessAsync(Process proc)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            tcs.Task.ContinueWith(t => Connected = t.Result);

            proc.Exited += (s, e) => tcs.TrySetResult(proc.ExitCode == 0);
            if (proc.HasExited) tcs.TrySetResult(proc.ExitCode == 0);

            return tcs.Task;
        }

        public abstract T Connect();
        public abstract T Disconnect();
        public abstract Task<T> ConnectAsync();
        public abstract Task<T> DisconnectAsync();
    }
}
