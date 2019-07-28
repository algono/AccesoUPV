using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors
{
    public abstract class ProcessConnector : Connectable
    {
        public abstract bool Connected { get; protected set; }

        public event EventHandler<ProcessEventArgs> ProcessConnected, ProcessDisconnected;

        protected ProcessStartInfo conInfo, disInfo;

        public ProcessConnector()
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
        protected virtual void OnProcessConnected(ProcessEventArgs e)
        {
            if (e.Succeeded) Connected = true;
            ProcessConnected?.Invoke(this, e);
        }
        protected abstract Process DisconnectProcess();
        protected virtual void OnProcessDisconnected(ProcessEventArgs e)
        {
            if (e.Succeeded) Connected = false;
            ProcessDisconnected?.Invoke(this, e);
        }

        public void Connect() => ConnectProcess().WaitAndCheck(OnProcessConnected);
        public async Task ConnectAsync() => await ConnectProcess().WaitAndCheckAsync(OnProcessConnected);
        public void Disconnect() => DisconnectProcess().WaitAndCheck(OnProcessDisconnected);
        public async Task DisconnectAsync() => await DisconnectProcess().WaitAndCheckAsync(OnProcessDisconnected);
    }
}
