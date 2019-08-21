using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    #region Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    [Serializable]
    public class NotAvailableDriveException : IOException
    {
        public string Drive { get; }

        public string DriveMessage => GetDriveMessage(Drive);

        public const string NoDriveMessage = "No existe ninguna unidad disponible. Libere alguna unidad y vuelva a intentarlo.";

        public NotAvailableDriveException() : base(NoDriveMessage)
        {

        }

        public NotAvailableDriveException(string drive) : base(GetDriveMessage(drive))
        {
            Drive = drive;
        }

        private static string GetDriveMessage(string drive)
            => $"La unidad definida para el disco ({drive}) ya contiene un disco asociado.\n\n"
            + "Antes de continuar, desconecte el disco asociado, o cambie la unidad utilizada para el disco.";
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
    #endregion

    public class NetworkDrive<T> : NetworkDrive where T : Enum
    {
        public IDictionary<T, DriveDomain> Domains { get; }

        private T _domain;
        public new T Domain
        {
            get => _domain;
            set
            {
                _domain = value;
                base.Domain = Domains[value];
            }
        }

        public NetworkDrive(Func<string, DriveDomain, string> getAddress, IDictionary<T, DriveDomain> domains, string drive = null, string user = null, string password = null) : base(getAddress, drive, null, user, password)
        {
            Domains = domains;
            base.Domain = Domains[Domain];
        }

    }

    public class NetworkDrive : ProcessConnector, Openable
    {
        private readonly Func<string, DriveDomain, string> _getAddress;
        public string Address => _getAddress(Username, Domain);
        public DriveDomain Domain { get; set; }

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

        public NetworkDrive(Func<string, DriveDomain, string> getAddress, string drive = null,
            DriveDomain domain = null, string user = null, string password = null)
        {
            _getAddress = getAddress;
            Domain = domain;

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

        public static List<string> GetDrives(bool onlyIfAvailable)
        {
            List<string> drives = new List<string>();
            List<string> mappedDrives = GetMappedDrives();

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                string drive = letter + ":";
                if (!(onlyIfAvailable && !IsAvailable(drive, mappedDrives)))
                {
                    drives.Add(drive);
                }
            }

            return drives;
        }

        public static IEnumerable<string> SelectAvailable(IEnumerable<string> drives)
        {
            List<string> mappedDrives = GetMappedDrives();
            return drives.Where((drive) => IsAvailable(drive, mappedDrives));
        }

        public static bool IsAvailable(string drive) => IsAvailable(drive, GetMappedDrives());

        private static bool IsAvailable(string drive, List<string> mappedDrives) 
            => !mappedDrives.Contains(drive)
               && !Directory.Exists(drive);

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
                List<string> availableDrives = GetDrives(onlyIfAvailable: true);
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