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

        private static Task<bool> WaitAsync(this Process process, bool verbose = false)
        {
            process.EnableRaisingEvents = true;

            var tcs = new TaskCompletionSource<bool>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode == 0);

            if (verbose)
            {
                process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
                process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
        #endregion

        #region Connectables Reflection
        /// <summary>
        /// <para>Selects only the properties of type <typeparamref name="T"/>, and retrieves its current values</para>
        /// The properties are selected using <seealso cref="WherePropertiesAreOfType)"/>
        /// </summary>
        public static IEnumerable<T> GetValuesOfType<T>(this IEnumerable<PropertyInfo> info, object obj) where T : class => info.WherePropertiesAreOfType<T>().GetValues<T>(obj);

        /// <summary>
        /// Selects only the properties of type <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<PropertyInfo> WherePropertiesAreOfType<T>(this IEnumerable<PropertyInfo> info) => info.Where(prop => typeof(T).IsAssignableFrom(prop.PropertyType));

        /// <summary>
        /// <para>Retrieves the current values of all the properties from <paramref name="info"/> in <paramref name="obj"/> as <typeparamref name="T"/></para>
        /// <para>Note: It assumes that all values are of type <typeparamref name="T"/>, or any of its subclasses</para>
        /// </summary>
        public static IEnumerable<T> GetValues<T>(this IEnumerable<PropertyInfo> info, object obj) where T : class => info.Select(prop => prop.GetValue(obj) as T);
        #endregion

        #region VPN
        private const string PSNameProperty = "Name";

        public static string GetStringPropertyValue(this PSObject obj, string propertyName) => (string)obj.Properties[propertyName].Value;

        public static string GetName(this PSObject obj) => obj.GetStringPropertyValue(PSNameProperty);
        #endregion

        #region WiFi Connections
        private const string NETSH = "netsh";
        private const string SHOW_INTERFACES = "wlan show interfaces";
        private static ProcessStartInfo ShowInterfacesStartInfo => new ProcessStartInfo()
        {
            FileName = NETSH,
            Arguments = SHOW_INTERFACES,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };
        private const string DISCONNECT_WIFI = "wlan disconnect";

        public static void ResetWiFiConnection(string connectionName)
        {
            Process.Start(NETSH, DISCONNECT_WIFI).WaitForExit();
            Process.Start(NETSH, GetConnectWiFi(connectionName)).WaitForExit();
        }

        public static async Task ResetWiFiConnectionAsync(string connectionName)
        {
            await Process.Start(NETSH, DISCONNECT_WIFI).WaitAsync();
            await Process.Start(NETSH, GetConnectWiFi(connectionName)).WaitAsync();
        }

        private static string GetConnectWiFi(string connectionName) => $"wlan connect {connectionName}";

        public static string IsAnyWiFiConnectionUp(IEnumerable<string> connectionNames)
        {
            string res = null;

            void HandleWiFiInterfaces(ProcessEventArgs proc)
            {
                if (proc.Succeeded)
                {
                    res = connectionNames.FirstOrDefault(connectionName => proc.Output.Contains(connectionName));
                }
            }

            Process.Start(ShowInterfacesStartInfo).WaitAndCheck(HandleWiFiInterfaces);

            return res;
        }

        public static async Task<string> IsAnyWiFiConnectionUpAsync(IEnumerable<string> connectionNames)
        {
            string res = null;

            void HandleWiFiInterfaces(ProcessEventArgs proc)
            {
                if (proc.Succeeded)
                {
                    res = connectionNames.FirstOrDefault(connectionName => proc.Output.Contains(connectionName));
                }
            }

            await Process.Start(ShowInterfacesStartInfo).WaitAndCheckAsync(HandleWiFiInterfaces);

            return res;
        }
        #endregion

    }
}
