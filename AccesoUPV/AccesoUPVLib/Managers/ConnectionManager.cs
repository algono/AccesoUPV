using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Managers
{
    public abstract class ConnectionManager : IConnectionManager
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
                RedirectStandardInput = true, //Por si tienes que pasarle algo al proceso
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
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

        public void Connect() => ConnectProcess().WaitAndCheck(ConnectionHandler);
        public async Task ConnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            await process.WaitAndCheckAsync(ConnectionHandler);
        }
        public void Disconnect() => DisconnectProcess().WaitAndCheck(DisconnectionHandler);
        public async Task DisconnectAsync()
        {
            Process process = await Task.Factory.StartNew(ConnectProcess);
            await process.WaitAndCheckAsync(DisconnectionHandler);
        }
    }
}
