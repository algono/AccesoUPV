using AccesoUPV.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    #region Custom Exceptions
    [Serializable]
    public class NotAvailableDriveException : IOException
    {
        public string DriveLetter => DriveLetterTools.ToDriveLetter(Letter);
        public char Letter { get; }

        public string DriveMessage => GetDriveMessage(DriveLetter);

        public const string NoDriveMessage = "No existe ninguna unidad disponible. Libere alguna unidad y vuelva a intentarlo.";

        public NotAvailableDriveException() : base(NoDriveMessage)
        {

        }

        public NotAvailableDriveException(char letter) : base(GetDriveMessage(DriveLetterTools.ToDriveLetter(letter)))
        {
            Letter = letter;
        }

        private static string GetDriveMessage(string letter)
            => $"La unidad definida para el disco ({letter}) ya contiene un disco asociado.\n\n"
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

        public NetworkDrive(Func<string, DriveDomain, string> getAddress,
            IDictionary<T, DriveDomain> domains,
            char drive = default,
            string user = null,
            string password = null) : base(getAddress, drive, domains[default], user, password)
        {
            Domains = domains;
        }

        public NetworkDrive(string address,
            IDictionary<T, DriveDomain> domains,
            char drive = default,
            string user = null,
            string password = null) : base(address, drive, domains[default], user, password)
        {
            Domains = domains;
        }

    }

    public class NetworkDrive : ProcessConnector, IOpenable, INameable
    {
        private readonly Func<string, DriveDomain, string> _getAddress;
        private readonly string _address;
        public string Address => _getAddress?.Invoke(Username, Domain) ?? _address;
        public DriveDomain Domain { get; set; }

        public string ConnectedDriveLetter
            => ConnectedLetter == default ? default : DriveLetterTools.ToDriveLetter(ConnectedLetter);
        public char ConnectedLetter { get; private set; }

        public string DriveLetter
            => Letter == default ? default : DriveLetterTools.ToDriveLetter(Letter);

        private char letter;
        private bool letterWasAutoAssigned;
        private bool needsUsername, needsPassword;

        public char Letter
        {
            get => letter;
            set
            {
                if (value == default || DriveLetterTools.IsValid(value))
                {
                    letter = value;
                    letterWasAutoAssigned = false;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(Letter), value, DriveLetterTools.InvalidLetterMessage);
                }
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public System.Security.SecureString SecurePassword { get; set; }
        public bool ExplicitUserArgument { get; set; }
        public bool AreCredentialsStored { get; set; }
        public bool NeedsUsername { get => !AreCredentialsStored && (needsUsername || ExplicitUserArgument); set => needsUsername = value; }
        public bool NeedsPassword { get => !AreCredentialsStored && needsPassword; set => needsPassword = value; }
        public bool YesToAll { get; set; }

        public string FullUsername => Domain?.GetFullUsername(Username) ?? Username;

        public override bool IsConnected
        {
            get => ConnectedLetter != default;
            protected set => ConnectedLetter = value ? Letter : default;
        }

        public string Name { get; set; }

        public char DefaultLetter { get; set; }

        private readonly ProcessStartInfo NetInfo = CreateProcessInfo("net.exe");

        public NetworkDrive(Func<string, DriveDomain, string> getAddress, char letter = default,
            DriveDomain domain = null, string user = null, string password = null) : this(address: null, letter, domain, user, password)
        {
            _getAddress = getAddress;
        }

        public NetworkDrive(string address, char letter = default,
            DriveDomain domain = null, string username = null, string password = null)
        {
            _address = address;
            Domain = domain;

            if (letter != default)
            {
                Letter = letter;
            }

            Username = username;
            Password = password;
            NeedsUsername = username != null;
            ExplicitUserArgument = false;
            NeedsPassword = password != null;
        }

        public override string ToString() => Name ?? Address;

        public void Open()
        {
            if (IsConnected) Process.Start(ConnectedDriveLetter);
            else throw new InvalidOperationException("El disco debe estar conectado para poder abrirlo");
        }

        #region Mapped Drives Static Methods
        public static IEnumerable<char> SelectAvailable(IEnumerable<char> drives)
        {
            List<char> mappedDrives = GetMappedDrives();
            return drives.Where((drive) => IsAvailable(drive, mappedDrives));
        }

        public static bool IsAvailable(char drive) => IsAvailable(drive, GetMappedDrives());

        private static bool IsAvailable(char drive, List<char> mappedDrives)
            => !mappedDrives.Contains(drive)
               && DriveLetterTools.IsAvailable(drive);

        public static List<char> GetMappedDrives()
        {
            List<char> drives = new List<char>();

            ProcessStartInfo info = CreateProcessInfo("net.exe");
            info.Arguments = "use";
            Process process = Process.Start(info);
            string output = process.StandardOutput.ReadToEnd();
            string[] splits = output.Split(':');
            for (int i = 0; i < splits.Length - 1; i++)
            {
                string split = splits[i];
                drives.Add(split[split.Length - 1]);
            }

            return drives;
        }
        #endregion

        #region Connection Process
        protected void CheckArguments()
        {
            if (NeedsUsername && string.IsNullOrEmpty(Username)) throw new ArgumentNullException(nameof(Username));
            if (NeedsPassword && string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password));

            if (!DriveLetterTools.IsValid(Letter))
            {
                letter = DriveLetterTools.GetFirstAvailable(prioritize: DefaultLetter);
                letterWasAutoAssigned = true;
            }
        }

        private void ApplyArguments()
        {
            NetInfo.Arguments = $"use {DriveLetter} {Address}";

            // The net use command doesn't allow to specify an user without a password
            if (NeedsPassword)
            {
                NetInfo.Arguments += $" \"{Password}\"";

                if (ExplicitUserArgument)
                {
                    NetInfo.Arguments += $" /USER:{FullUsername}";
                }
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
            CheckArguments();
            ApplyArguments();
            return StartProcess(NetInfo);
        }

        protected override void OnProcessConnected(ProcessEventArgs e)
        {
            base.OnProcessConnected(e);

            if (letterWasAutoAssigned)
            {
                letter = default;
                letterWasAutoAssigned = false;
            }

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


                if (e.OutputOrErrorContains("85"))
                    throw new NotAvailableDriveException(Letter);
            }
        }

        protected override Process DisconnectProcess()
        {
            NetInfo.Arguments = $"use {ConnectedDriveLetter} /delete";
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
        #endregion
    }
}