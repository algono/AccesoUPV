using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    // Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    [Serializable]
    public class NotAvailableDriveException : IOException
    {
    }

    [Serializable]
    public class OpenedFilesException : IOException
    {
        public const string WarningTitle = "Archivos abiertos";

        public const string WarningMessage =
            "Existen archivos abiertos y/o búsquedas incompletas de directorios pendientes en el disco. Si no los cierra antes de desconectarse, podría perder datos.\n\n"
            + "¿Desea continuar la desconexión y forzar el cierre?";

        public Action Continue { get; }
        public Func<Task> ContinueAsync { get; }

        public OpenedFilesException(Action continueMethod, Func<Task> continueMethodAsync = null) : base(WarningMessage)
        {
            Continue = continueMethod;
            ContinueAsync = continueMethodAsync;
        }
    }

    public class NetworkDrive<T> : NetworkDrive where T : Enum
    {
        public NetworkDriveConfig<T> Config { get; }
        public override string Address => Config.GetAddress(Username, base.Domain);
        public new T Domain { get; set; }
        protected override DriveDomain DriveDomain => Config.GetDriveDomain(Domain);

        public NetworkDrive(NetworkDriveConfig<T> config, T defaultDomain,
            string drive = null, string user = null, string password = null) : base(null, null, drive, user, password)
        {
            Config = config;
            Domain = defaultDomain;
        }
    }

    public class NetworkDrive : ProcessConnector, Openable
    {
        private readonly Func<string, DriveDomain, string> _getAddress;
        public virtual string Address => _getAddress(Username, Domain);
        public DriveDomain Domain => DriveDomain;
        protected virtual DriveDomain DriveDomain { get; }

        public string ConnectedDrive { get; private set; }
        public string Drive { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseCredentials { get; set; }
        public bool YesToAll { get; set; }

        public override bool IsConnected
        {
            get => ConnectedDrive != null;
            protected set => ConnectedDrive = value ? Drive : null;
        }

        private static readonly ProcessStartInfo NetInfo = CreateProcessInfo("net.exe");

        public NetworkDrive(Func<string, DriveDomain, string> getAddress, DriveDomain domain,
            string drive = null, string user = null, string password = null)
        {
            _getAddress = getAddress;
            DriveDomain = domain;

            Drive = drive;
            Username = user;
            Password = password;
            UseCredentials = password != null;
        }

        public void Open()
        {
            if (IsConnected) Process.Start(ConnectedDrive);
            else throw new InvalidOperationException("El disco debe estar conectado para poder abrirlo");
        }

        public static List<string> GetAvailableDrives()
        {
            List<string> drives = new List<string>();
            List<string> mappedDrives = GetMappedDrives();

            for (char letter = 'Z'; letter >= 'D'; letter--)
            {
                string drive = letter + ":";
                if (!Directory.Exists(drive) && !mappedDrives.Contains(drive)) drives.Add(drive);
            }

            return drives;
        }

        public static List<string> GetMappedDrives()
        {
            List<string> drives = new List<string>();

            ProcessStartInfo info = CreateProcessInfo("net.exe");
            info.Arguments = "use";
            Process process = Process.Start(info);
            string output = process.StandardOutput.ReadToEnd();
            string[] splits = output.Split(':');
            for (int i = 0; i < splits.Length - 1; i++)
            {
                string split = splits[i];
                drives.Add(split[split.Length - 1] + ":");
            }

            return drives;
        }

        protected void CheckDrive()
        {
            if (string.IsNullOrEmpty(Drive))
            {
                List<string> availableDrives = GetAvailableDrives();
                if (availableDrives.Count == 0) throw new NotAvailableDriveException();
                Drive = availableDrives[0];
            }
            else if (Directory.Exists(Drive))
            {
                throw new NotAvailableDriveException();
            }
        }

        protected void CheckArguments()
        {
            if (string.IsNullOrEmpty(Username)) throw new ArgumentNullException(nameof(Username));

            NetInfo.Arguments = $"use {Drive} {Address}";
            if (UseCredentials)
            {
                if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password));
                NetInfo.Arguments += $" \"{Password}\" /USER:{Domain?.GetFullUsername(Username) ?? Username}";
            }

            if (YesToAll) NetInfo.Arguments += " /y";
        }

        protected Process StartProcess(ProcessStartInfo info)
        {
            Process process = Process.Start(info);
            process?.StandardInput.Close(); //Para que falle si pide input al usuario
            return process;
        }

        protected override Process ConnectProcess()
        {
            CheckDrive();
            CheckArguments();
            return StartProcess(NetInfo);
        }

        protected override void OnProcessConnected(ProcessEventArgs e)
        {
            base.OnProcessConnected(e);

            if (!e.Succeeded)
            {
                // 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dirección no es valida).
                if (e.OutputOrErrorContains("55"))
                    throw new ArgumentOutOfRangeException(nameof(Address));

                /**
                * 86 - Error del sistema "La contraseña de red es incorrecta"
                * 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
                * Cuando las credenciales son erróneas, da uno de estos dos errores de forma arbitraria.
                * 
                * Suponemos que en el primer caso el error fue de la contraseña y en el segundo del usuario,
                * pero no lo sabemos con seguridad.
                */

                if (e.OutputOrErrorContains("86"))
                    throw new ArgumentException(e.Error, nameof(Password));
                if (e.OutputOrErrorContains("1326"))
                    throw new ArgumentException(e.Error, nameof(Username));
            }
        }

        protected override Process DisconnectProcess()
        {
            NetInfo.Arguments = $"use {ConnectedDrive} /delete";
            if (YesToAll) NetInfo.Arguments += " /y";
            return StartProcess(NetInfo);
        }

        protected override void OnProcessDisconnected(ProcessEventArgs e)
        {
            base.OnProcessDisconnected(e);

            if (!e.Succeeded)
                //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
                if (e.OutputOrErrorContains("/N)"))
                    throw new OpenedFilesException(ForceDisconnect, ForceDisconnectAsync);
        }

        private void ForceDisconnect()
        {
            bool oldYesToAll = YesToAll;
            YesToAll = true;
            Disconnect();
            YesToAll = oldYesToAll;
        }

        private async Task ForceDisconnectAsync()
        {
            bool oldYesToAll = YesToAll;
            YesToAll = true;
            await DisconnectAsync();
            YesToAll = oldYesToAll;
        }
    }
}