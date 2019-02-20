using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Library
{
    public static class Utilities
    {
        public static void WaitAndCheck(this Process process, Action<bool, string, string> handler = null)
        {
            string output = "", error = "";
            if (!process.StartInfo.UseShellExecute)
            {
                if (process.StartInfo.RedirectStandardOutput) output = process.StandardOutput.ReadToEnd();
                if (process.StartInfo.RedirectStandardError) error = process.StandardError.ReadToEnd();
            }
            
            process.WaitForExit();

            bool succeeded = process.ExitCode == 0;

            process.Close();

            handler?.Invoke(succeeded, output, error);

            if (!succeeded) throw new IOException($"Output:\n{output}\n\nError:\n{error}");
        }

        public static async Task WaitAndCheckAsync(this Process process, Action<bool, string, string> handler = null)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Task<string> outputTask = null, errorTask = null;
            HashSet<Task> tasks = new HashSet<Task>
            {
                tcs.Task
            };

            if (!process.StartInfo.UseShellExecute)
            {
                if (process.StartInfo.RedirectStandardOutput)
                {
                    outputTask = process.StandardOutput.ReadToEndAsync();
                    tasks.Add(outputTask);
                }
                if (process.StartInfo.RedirectStandardError)
                {
                    errorTask = process.StandardError.ReadToEndAsync();
                    tasks.Add(errorTask);
                }
            }

            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode == 0);
            if (process.HasExited) tcs.TrySetResult(process.ExitCode == 0);

            await Task.WhenAll(tasks);
            process.Close();

            bool succeeded = tcs.Task.Result;

            if (handler != null) await Task.Run(() => handler(succeeded, outputTask?.Result ?? "", errorTask?.Result ?? ""));

            if (!succeeded) throw new IOException($"Output:\n{outputTask?.Result}\n\nError:\n{errorTask?.Result}");
        }
    }
}
