using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.VPN
{
    public abstract class VPNBase : ProcessConnector, IVPN
    {
        public const int CONNECTED_PING_TIMEOUT = 5000, DISCONNECTED_PING_TIMEOUT = 500;

        public string ConnectedName { get; private set; }
        public string Name { get; set; }
        public abstract string Server { get; }
        public abstract string TestServer { get; }
        public override bool Connected
        {
            get => ConnectedName != null;
            protected set => ConnectedName = value ? Name : null;
        }

        protected static readonly ProcessStartInfo PingInfo = CreateProcessInfo("ping.exe");

        protected VPNBase(string name = null, bool findNameAuto = false)
        {
            if (name == null && findNameAuto) SetNameAuto();
            else Name = name;

            conInfo.FileName = "rasphone.exe";
            disInfo.FileName = "rasdial.exe";
        }

        public bool IsReachable()
        {
            return IsReachable(Connected ? CONNECTED_PING_TIMEOUT : DISCONNECTED_PING_TIMEOUT);
        }

        public bool IsReachable(int timeout)
        {
            if (string.IsNullOrEmpty(TestServer)) throw new ArgumentNullException("The test server is not defined.");
            PingInfo.Arguments = $"-n 1 -w {timeout} {TestServer}";
            Process p = Process.Start(PingInfo);
            p.WaitForExit();
            return p.ExitCode == 0;
        }

        public void Refresh()
        {
            Connected = IsActuallyConnected();
        }

        protected override Process ConnectProcess()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("The name is not defined.");
            conInfo.Arguments = $"-d \"{Name}\"";
            return Process.Start(conInfo);
        }
        /**
         * @throws:
         * - ArgumentException: La VPN que se ha proporcionado funciona, pero es incapaz de acceder al Test Server
         * - OperationCanceledException: El usuario canceló la operación.
         */
        protected override void ConnectionHandler(bool succeeded, string output, string error)
        {
            if (succeeded)
            {
                try
                {
                    if (!IsActuallyConnected()) throw new OperationCanceledException();

                    if (!IsReachable(CONNECTED_PING_TIMEOUT))
                    {
                        disInfo.Arguments = $"\"{Name}\" /DISCONNECT";
                        Process.Start(disInfo).WaitAndCheck();
                        throw new ArgumentException(); //VPN no valida para acceder al TestServer
                    }
                }
                catch (IOException)
                {
                    //If the checking fails, it still continues
                }
            }
            
            base.ConnectionHandler(succeeded, output, error);

        }

        private bool IsActuallyConnected()
        {
            bool res = false;
            Process checkingProcess = Process.Start(CreateProcessInfo("rasdial.exe"));
            checkingProcess.WaitAndCheck((s, o, e) =>
            {
                res = s && o.Contains(Name);
            });
            return res;
        }

        protected override Process DisconnectProcess()
        {
            disInfo.Arguments = $"\"{ConnectedName}\" /DISCONNECT";
            return Process.Start(disInfo);
        }

        protected virtual PowerShell CreateShell()
        {
            if (string.IsNullOrEmpty(Name)) throw new ArgumentNullException("The name is not defined.");
            else if (string.IsNullOrEmpty(Server)) throw new ArgumentNullException("The server is not defined.");

            PowerShell shell = PowerShell.Create();
            shell.AddCommand("Add-VpnConnection");
            shell.AddParameter("Name", Name);
            shell.AddParameter("ServerAddress", Server);
            //Es necesario para que las credenciales se guarden cuando el usuario lo indique en rasphone
            shell.AddParameter("RememberCredential");
            return shell;
        }

        public bool Create()
        {
            using (PowerShell shell = CreateShell())
            {
                shell.Invoke();
                return !shell.HadErrors;
            }
        }
        public Task CreateAsync()
        {
            PowerShell shell = CreateShell();
            return new TaskFactory().FromAsync(shell.BeginInvoke(), (res) => shell.Dispose());
        }

        public bool SetNameAuto()
        {
            List<PSObject> vpnList = Find();
            if (vpnList.Count <= 0) return false;
            Name = vpnList[0].GetStringPropertyValue("Name");
            return true;
        }

        public bool Exists() => Find(Server).Exists(vpn => vpn.GetStringPropertyValue("Name") == Name);

        public List<PSObject> Find() => Find(Server);

        public static List<PSObject> Find(string server)
        {
            using (PowerShell shell = PowerShell.Create())
            {
                shell.AddScript("Get-VpnConnection | Where-Object {$_.ServerAddress -eq '" + server + "'}");
                List<PSObject> PSOutput = shell.Invoke().ToList();
                PSOutput.RemoveAll(item => item == null);
                return PSOutput;
            }
        }
    }
}
