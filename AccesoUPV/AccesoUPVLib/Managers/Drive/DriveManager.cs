using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    // Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    public class InvalidUserException : ArgumentException { }
    public class NotAvailableDriveException : IOException { }
    public class OpenedFilesException : IOException { }
    public abstract class DriveManager : ConnectionManager
    {
        public char? ConnectedDrive { get; private set; }
        public char? Drive { get; set; }
        public abstract string Address { get; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
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

        public DriveManager(char? drive = null, string user = null, string password = null, string domain = null, bool useCredentials = false, bool yesToAll = false) : base()
        {
            Drive = drive;
            User = user;
            Password = password;
            Domain = domain;
            UseCredentials = useCredentials;
            YesToAll = yesToAll;

            conInfo.FileName = "net.exe";
            disInfo.FileName = "net.exe";
        }

        public static List<char> GetAvailableDrives()
        {
            List<char> drives = new List<char>();
            
            for (char letter = 'Z'; letter >= 'D'; letter--)
            {
                if (!Directory.Exists(letter + ":")) drives.Add(letter);
            }

            return drives;
        }

        protected override Process ConnectProcess()
        {
            if (Drive == null)
            {
                List<char> availableDrives = GetAvailableDrives();
                if (availableDrives.Count == 0) throw new NotAvailableDriveException();
                Drive = availableDrives[0];
            }
            else if (Directory.Exists(Drive + ":"))
            {
                throw new NotAvailableDriveException();
            }

            conInfo.Arguments = $"use {Drive}: {Address}";
            if (UseCredentials) conInfo.Arguments += $" {Password} /USER:{Domain}\\{User}";

            Process process = Process.Start(conInfo);
            process.StandardInput.Close(); //Para que falle si pide input al usuario
            return process;
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
            disInfo.Arguments = $"use {ConnectedDrive}: /delete";
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
