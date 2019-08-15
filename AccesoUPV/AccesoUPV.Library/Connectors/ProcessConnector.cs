using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public abstract class ProcessConnector : Connectable<ProcessEventArgs>
    {
        public abstract bool IsConnected { get; protected set; }

        public event EventHandler<ProcessEventArgs> Connected, Disconnected;

        public static ProcessStartInfo CreateProcessInfo(string fileName)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true, //Por si le tienes que pasar algo al proceso
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        protected abstract Process ConnectProcess();
        protected virtual void OnProcessConnected(ProcessEventArgs e)
        {
            if (e.Succeeded) IsConnected = true;
            Connected?.Invoke(this, e);
        }
        protected abstract Process DisconnectProcess();
        protected virtual void OnProcessDisconnected(ProcessEventArgs e)
        {
            if (e.Succeeded) IsConnected = false;
            Disconnected?.Invoke(this, e);
        }

        public void Connect() => ConnectProcess().WaitAndCheck(OnProcessConnected);
        public async Task ConnectAsync() => await ConnectProcess().WaitAndCheckAsync(OnProcessConnected);
        public void Disconnect() => DisconnectProcess().WaitAndCheck(OnProcessDisconnected);
        public async Task DisconnectAsync() => await DisconnectProcess().WaitAndCheckAsync(OnProcessDisconnected);
    }
}
