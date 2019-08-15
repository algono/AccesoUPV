using AccesoUPV.Library.Connectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading.Tasks;

namespace AccesoUPV.Library
{
    public static class Utilities
    {

        #region Process
        public static void WaitAndCheck(this Process process, Action<ProcessEventArgs> handler = null)
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

            handler?.Invoke(new ProcessEventArgs(succeeded, output, error));

            ThrowExceptionIfNotSucceeded(succeeded, output, error);
        }

        public static async Task WaitAndCheckAsync(this Process process, Action<ProcessEventArgs> handler = null)
        {
            string output = "", error = "";

            process.OutputDataReceived += (s, ea) => output += ea.Data;
            process.ErrorDataReceived += (s, ea) => error += ea.Data;

            bool succeeded = await process.WaitAsync().ConfigureAwait(false);

            process.Close();

            handler?.Invoke(new ProcessEventArgs(succeeded, output, error)); // TODO: Convertir llamada a async

            ThrowExceptionIfNotSucceeded(succeeded, output, error);
        }

        private static void ThrowExceptionIfNotSucceeded(bool succeeded, string output, string error)
        {
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
        #endregion

        #region Connectables Reflection
        public static IEnumerable<T> GetValuesOfType<T>(this IEnumerable<PropertyInfo> info, object obj) where T : class => info.WherePropertiesAreOfType<T>().GetValues<T>(obj);
        public static IEnumerable<PropertyInfo> WherePropertiesAreOfType<T>(this IEnumerable<PropertyInfo> info) => info.Where(prop => typeof(T).IsAssignableFrom(prop.PropertyType));
        public static IEnumerable<T> GetValues<T>(this IEnumerable<PropertyInfo> info, object obj) where T : class => info.Select(prop => prop.GetValue(obj) as T);
        #endregion

        #region VPN
        public static string GetStringPropertyValue(this PSObject obj, string propertyName) => (string)obj.Properties[propertyName].Value;
        #endregion

    }
}
