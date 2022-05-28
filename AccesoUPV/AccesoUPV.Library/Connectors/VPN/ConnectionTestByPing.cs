using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public class ConnectionTestByPing : IConnectionTestMethod
    {
        private const string PING_FILENAME = "ping.exe";

        public string TestServer { get; set; }

        public ConnectionTestByPing(string testServer)
        {
            TestServer = testServer;
        }

        public string GetErrorMessage(string networkName) => $"No se pudo conectar al servidor de prueba de la red \"{networkName}\": {TestServer}";

        public bool WaitUntilReachable(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000)
        {
            bool reachable = false;
            int requestsMade = 0;
            while (!reachable && requestsMade < maxRequests)
            {
                Thread.Sleep(millisPerRequest);
                reachable = IsReachable(requestTimeout);
                requestsMade++;
            }

            return reachable;
        }

        public async Task<bool> WaitUntilReachableAsync(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000)
        {
            bool reachable = false;
            int requestsMade = 0;
            while (!reachable && requestsMade < maxRequests)
            {
                await Task.Delay(millisPerRequest);
                reachable = await IsReachableAsync(requestTimeout);
                requestsMade++;
            }

            return reachable;
        }

        public bool IsReachable(int timeout)
        {
            Process p = StartPing(timeout);
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        public async Task<bool> IsReachableAsync(int timeout)
        {
            Process p = StartPing(timeout);
            return (await p.WaitAndCheckAsync()).Succeeded;
        }

        private Process StartPing(int timeout)
        {
            if (string.IsNullOrEmpty(TestServer)) throw new ArgumentNullException(nameof(TestServer));

            ProcessStartInfo pingInfo = ProcessConnector.CreateProcessInfo(PING_FILENAME);
            pingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            return Process.Start(pingInfo);
        }
    }
}
