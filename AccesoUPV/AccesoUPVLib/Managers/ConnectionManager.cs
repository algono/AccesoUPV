using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers
{
    public abstract class ConnectionManager
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

        protected bool CheckProcess(Process proc, bool shouldBeConnectedAfter)
        {
            proc.WaitForExit();
            if (proc.ExitCode == 0)
            {
                Connected = shouldBeConnectedAfter;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Task<bool> CheckProcessAsync(Process proc)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            tcs.Task.ContinueWith(t => Connected = t.Result);

            proc.Exited += (s, e) => tcs.TrySetResult(proc.ExitCode == 0);
            if (proc.HasExited) tcs.TrySetResult(proc.ExitCode == 0);

            return tcs.Task;
        }

        public abstract bool Connect();
        public abstract bool Disconnect();
        public abstract Task<bool> ConnectAsync();
        public abstract Task<bool> DisconnectAsync();
    }
}
