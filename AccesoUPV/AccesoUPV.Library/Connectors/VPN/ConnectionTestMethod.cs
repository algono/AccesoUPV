using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public interface IConnectionTestMethod
    {
        bool IsReachable(int timeout);

        Task<bool> IsReachableAsync(int timeout);

        bool WaitUntilReachable(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000);

        Task<bool> WaitUntilReachableAsync(int requestTimeout, int maxRequests = 5, int millisPerRequest = 1000);

        string GetErrorMessage(string networkName);
    }
}
