using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public static class ProcessUtils
    {
        public static bool WaitAndCheck(this Process process, Action<bool, string, string> handler = null)
        {
            string output = process.StartInfo.RedirectStandardOutput ? process.StandardOutput.ReadToEnd() : "";
            string err = process.StartInfo.RedirectStandardError ? process.StandardError.ReadToEnd() : "";

            process.WaitForExit();

            bool succeeded = process.ExitCode == 0;

            process.Close();

            handler?.Invoke(succeeded, output, err);

            return succeeded;
        }

        public static async Task<bool> WaitAndCheckAsync(this Process process, Action<bool, string, string> handler = null)
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
    }
}
