using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
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

            handler?.Invoke(succeeded, output, error); // TODO: Convertir llamada a async

            if (!succeeded) throw new IOException($"Output:\n{output}\n\nError:\n{error}");
        }

        public static async Task WaitAndCheckAsync(this Process process, Action<bool, string, string> handler = null)
        {
            string output = "", error = "";

            process.OutputDataReceived += (s, ea) => output += ea.Data;
            process.ErrorDataReceived += (s, ea) => error += ea.Data;

            bool succeeded = await process.WaitAsync();

            handler?.Invoke(succeeded, output, error);

            if (!succeeded) throw new IOException($"Output:\n{output}\n\nError:\n{error}");
        }

        public static async Task<bool> RunProcessAsync(string fileName, string args)
        {
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName, Arguments = args,
                    UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true
                },
                EnableRaisingEvents = true
            })
            {
                process.Start();
                return await process.WaitAsync().ConfigureAwait(false);
            }
        }

        private static Task<bool> WaitAsync(this Process process)
        {
            process.EnableRaisingEvents = true;

            var tcs = new TaskCompletionSource<bool>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode == 0);
            process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
            process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        public static string GetStringPropertyValue(this PSObject obj, string propertyName) => (string)obj.Properties["Name"].Value;

    }
}
