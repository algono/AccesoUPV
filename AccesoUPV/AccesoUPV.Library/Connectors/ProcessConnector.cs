using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public abstract class ProcessConnector : IConnectable
    {
        public abstract bool IsConnected { get; protected set; }

        public event EventHandler Connected, Disconnected;
        public event EventHandler<ProcessEventArgs> ProcessConnected, ProcessDisconnected;
        public event PropertyChangedEventHandler PropertyChanged;

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

        private void OnConnectionStatusChanged() => OnPropertyChanged(nameof(IsConnected));

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected abstract Process ConnectProcess();
        protected virtual void OnProcessConnected(ProcessEventArgs e)
        {
            ProcessConnected?.Invoke(this, e);

            if (e.Succeeded)
            {
                IsConnected = true;
                OnConnectionStatusChanged();
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
                OnConnectionStatusChanged();
                Disconnected?.Invoke(this, e);
            }

        }

        public void Connect() => ConnectProcess().WaitAndCheck(OnProcessConnected);
        public async Task ConnectAsync() => await ConnectProcess().WaitAndCheckAsync(OnProcessConnected);
        public void Disconnect() => DisconnectProcess().WaitAndCheck(OnProcessDisconnected);
        public async Task DisconnectAsync() => await DisconnectProcess().WaitAndCheckAsync(OnProcessDisconnected);
    }
}
