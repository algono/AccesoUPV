using System;
using System.Diagnostics;
using System.IO;
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

        protected static ProcessStartInfo CreateProcessInfo(string fileName = null)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        protected bool CheckProcess(Process process, Action<bool, string, string> handler = null)
        {
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();

            process.WaitForExit();

            bool succeeded = process.ExitCode == 0;

            handler?.Invoke(succeeded, output, err);

            process.Close();

            return succeeded;
        }

        protected async Task<bool> CheckProcessAsync(Process process, Action<bool, string, string> handler = null)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();

            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode == 0);
            if (process.HasExited) tcs.TrySetResult(process.ExitCode == 0);

            await Task.WhenAll(output, error, tcs.Task);
            process.Close();

            bool succeeded = tcs.Task.Result;

            if (handler != null) await Task.Run(() => handler(succeeded, output.Result, error.Result));

            if (!succeeded) throw new IOException($"Output:\n{output}\n\nError:\n{error}");

            return succeeded;
        }

        protected abstract Process ConnectProcess();
        protected virtual void ConnectionHandler(bool succeeded, string output, string error)
        {
            if (succeeded) Connected = true;
        }
        protected abstract Process DisconnectProcess();
        protected virtual void DisconnectionHandler(bool succeeded, string output, string error)
        {
            if (succeeded) Connected = false;
        }

        public void Connect() => CheckProcess(ConnectProcess(), ConnectionHandler);
        public async Task ConnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            await CheckProcessAsync(process, ConnectionHandler);
        }
        public void Disconnect() => CheckProcess(DisconnectProcess(), DisconnectionHandler);
        public async Task DisconnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            await CheckProcessAsync(process, DisconnectionHandler);
        }
    }
}
