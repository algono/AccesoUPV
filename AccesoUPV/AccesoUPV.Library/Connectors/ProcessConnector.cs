using AccesoUPV.Library.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public abstract class ProcessConnector : IConnectable
    {
        public abstract bool IsConnected { get; protected set; }

        public event EventHandler Connected, Disconnected;
        public event EventHandler<ProcessEventArgs> ProcessConnected, ProcessDisconnected;

        public static ProcessStartInfo CreateProcessInfo(string fileName)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true, // Por si le tienes que pasar algo al proceso
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }

        protected abstract Process ConnectProcess();
        protected virtual void OnProcessConnected(ProcessEventArgs e)
        {
            ProcessConnected?.Invoke(this, e);

            if (e.Succeeded)
            {
                IsConnected = true;
                Connected?.Invoke(this, e);
            }

        }
        protected abstract Process DisconnectProcess();
        protected virtual void OnProcessDisconnected(ProcessEventArgs e)
        {
            ProcessDisconnected?.Invoke(this, e);

            if (e.Succeeded)
            {
                IsConnected = false;
                Disconnected?.Invoke(this, e);
            }

        }

        public void Connect() => ConnectProcess().WaitAndCheck(OnProcessConnected);
        public async Task ConnectAsync() => await ConnectProcess().WaitAndCheckAsync(OnProcessConnected);
        public void Disconnect() => DisconnectProcess().WaitAndCheck(OnProcessDisconnected);
        public async Task DisconnectAsync() => await DisconnectProcess().WaitAndCheckAsync(OnProcessDisconnected);
    }
}
