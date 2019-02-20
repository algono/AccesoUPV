using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    // Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    public class InvalidUserException : ArgumentException { }
    public class NotAvailableDriveException : IOException { }
    public class OpenedFilesException : IOException { }
    public abstract class NetworkDriveBase : ProcessConnector, INetworkDrive
    {
        public string ConnectedDrive { get; private set; }
        public string Drive { get; set; }
        public abstract string Address { get; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseCredentials { get; set; }
        public bool YesToAll { get; set; }

        public override bool Connected
        {
            get { return ConnectedDrive != null; }
            protected set
            {
                if (value) ConnectedDrive = Drive;
                else ConnectedDrive = null;
            }
        }

        public virtual DriveDomain Domain { get; protected set; }

        public NetworkDriveBase(string drive = null, string user = null, string password = null, bool useCredentials = false, bool yesToAll = false) : base()
        {
            Drive = drive;
            UserName = user;
            Password = password;
            UseCredentials = useCredentials;
            YesToAll = yesToAll;

            conInfo.FileName = "net.exe";
            disInfo.FileName = "net.exe";
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
            conInfo.Arguments = $"use {Drive} {Address}";
            if (UseCredentials)
            {
                if (string.IsNullOrEmpty(UserName)) throw new ArgumentNullException("UserName is not set");
                if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException("Password is not set");
                conInfo.Arguments += $" \"{Password}\" /USER:{Domain?.GetFullUserName(UserName) ?? UserName}";
            }
            if (YesToAll) conInfo.Arguments += "/y";
        }

        protected Process StartProcess()
        {
            Process process = Process.Start(conInfo);
            process.StandardInput.Close(); //Para que falle si pide input al usuario
            return process;
        }

        protected override Process ConnectProcess()
        {
            CheckDrive();
            CheckArguments();
            return StartProcess();
        }

        protected override void ConnectionHandler(bool succeeded, string output, string error)
        {
            base.ConnectionHandler(succeeded, output, error);

            if (!succeeded)
            { 
                // 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dir no existe, por tanto, el usuario no es válido).
                if (output.Contains("55") || error.Contains("55"))
                {
                    throw new InvalidUserException();
                }
            }
        }

        protected override Process DisconnectProcess()
        {
            disInfo.Arguments = $"use {ConnectedDrive} /delete";
            if (YesToAll) disInfo.Arguments += "/y";
            return Process.Start(disInfo);
        }

        protected override void DisconnectionHandler(bool succeeded, string output, string error)
        {
            base.DisconnectionHandler(succeeded, output, error);

            if (!succeeded)
            {
                //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
                if (output.Contains("/N)") || error.Contains("/N)"))
                {
                    throw new OpenedFilesException();
                }
            }
        }

    }
}
