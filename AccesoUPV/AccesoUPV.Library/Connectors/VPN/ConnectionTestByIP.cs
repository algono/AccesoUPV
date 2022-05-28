using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class ConnectionTestByIP : IConnectionTestMethod
    {
        public string Prefix { get; }

        public ConnectionTestByIP(string prefix)
        {
            Prefix = prefix;
        }

        public string GetErrorMessage(string networkName) => $"La conexión no forma parte de la red \"{networkName}\" (Su IP no comienza con: {Prefix})";

        public bool IsReachable(int _) => GetIPAddresses().Any(ip => ip.StartsWith(Prefix));

        public Task<bool> IsReachableAsync(int timeout) => Task.Run(() => IsReachable(timeout));

        public bool WaitUntilReachable(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000)
        {
            for (int i = 0; i < maxRequests; i++)
            {
                bool result = IsReachable(requestTimeout);
                if (result) return result;

                Thread.Sleep(millisPerRequest);
            }
            return false;
        }

        public async Task<bool> WaitUntilReachableAsync(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000)
        {
            for (int i = 0; i < maxRequests; i++)
            {
                bool result = await IsReachableAsync(requestTimeout);
                if (result) return result;

                await Task.Delay(millisPerRequest);
            }
            return await Task.FromResult(false);
        }

        private IEnumerable<NetworkInterface> GetNetworkInterfaces() => NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.OperationalStatus == OperationalStatus.Up);
        private IEnumerable<string> GetIPAddresses() => GetNetworkInterfaces()
            .Select(ni =>
                ni.GetIPProperties().UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => ip.Address.ToString()))
            .SelectMany(i => i); // Flatten
    }
}
