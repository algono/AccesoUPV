using System;
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
            conInfo = CreateProcessInfo();
            disInfo = CreateProcessInfo();
        }

        private static ProcessStartInfo CreateProcessInfo()
        {
            return new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        protected bool CheckProcess(Process process, bool shouldBeConnectedAfter, Action<string, string> handler = null)
        {
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();

            handler?.Invoke(output, err);

            process.WaitForExit();

            bool succeeded = process.ExitCode == 0;

            if (succeeded) Connected = shouldBeConnectedAfter;
            process.Close();

            return succeeded;
        }

        protected Task<bool> CheckProcessAsync(Process process, bool shouldBeConnectedAfter)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            tcs.Task.ContinueWith(t => {
                if (t.Result) Connected = shouldBeConnectedAfter;
                process.Close();
            });

            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode == 0);
            if (process.HasExited) tcs.TrySetResult(process.ExitCode == 0);

            return tcs.Task;
        }

        protected async Task<bool> CheckProcessAsync(Process process, bool shouldBeConnectedAfter, Action<string, string> handler)
        {
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> err = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(output, err);
            await Task.Run(() => handler(output.Result, err.Result));

            return await CheckProcessAsync(process, shouldBeConnectedAfter);
        }

        protected abstract Process ConnectProcess();
        protected abstract void ConnectionHandler(string output, string err);
        protected abstract Process DisconnectProcess();
        protected abstract void DisconnectionHandler(string output, string err);

        public bool Connect() => CheckProcess(ConnectProcess(), true, ConnectionHandler);
        public async Task<bool> ConnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            return await CheckProcessAsync(process, true, ConnectionHandler);
        }
        public bool Disconnect() => CheckProcess(DisconnectProcess(), false, DisconnectionHandler);
        public async Task<bool> DisconnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            return await CheckProcessAsync(process, false, DisconnectionHandler);
        }
    }
}
