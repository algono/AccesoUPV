using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Connectors.VPN;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;

namespace AccesoUPV.Library
{
    public static class Utilities
    {

        #region Process
        public static void WaitAndCheck(this Process process, Action<ProcessEventArgs> handler = null)
        {
            ProcessEventArgs result = WaitAndCheck(process);

            handler?.Invoke(result);

            ThrowExceptionIfNotSucceeded(result);
        }

        public static ProcessEventArgs WaitAndCheck(this Process process)
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

            return new ProcessEventArgs(succeeded, output, error);
        }

        public static async Task WaitAndCheckAsync(this Process process, Action<ProcessEventArgs> handler)
        {
            ProcessEventArgs result = await WaitAndCheckAsync(process);

            handler?.Invoke(result); // TODO: Convertir llamada a async

            ThrowExceptionIfNotSucceeded(result);
        }

        private static void ThrowExceptionIfNotSucceeded(ProcessEventArgs args)
        {
            if (!args.Succeeded) throw new IOException($"Output:\n{args.Output}\n\nError:\n{args.Error}");
        }

        public static async Task<ProcessEventArgs> WaitAndCheckAsync(this Process process)
        {
            string output = "", error = "";

            process.OutputDataReceived += (s, ea) => output += ea.Data;
            process.ErrorDataReceived += (s, ea) => error += ea.Data;

            bool succeeded = await process.WaitAsync().ConfigureAwait(false);

            process.Close();

            return new ProcessEventArgs(succeeded, output, error);
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
        private const string DISCONNECT_WIFI = "wlan disconnect";

        private static ProcessStartInfo ConnectStartInfo(string connectionName) => ProcessConnector.CreateProcessInfo(NETSH, $"wlan connect {connectionName}");
        private static ProcessStartInfo DisconnectStartInfo => ProcessConnector.CreateProcessInfo(NETSH, DISCONNECT_WIFI);
        

        public static void ResetWiFiConnection(string connectionName)
        {
            Process.Start(DisconnectStartInfo).WaitForExit();
            Process.Start(ConnectStartInfo(connectionName)).WaitForExit();
        }

        public static async Task ResetWiFiConnectionAsync(string connectionName)
        {
            await Process.Start(DisconnectStartInfo).WaitAndCheckAsync();
            await Process.Start(ConnectStartInfo(connectionName)).WaitAndCheckAsync();
        }

        public static IEnumerable<NetworkInterface> GetValidWiFiInterfaces(this ConnectionTestByIP test)
        {
            return test.GetValidInterfaces().Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
        }
        #endregion

    }
}
