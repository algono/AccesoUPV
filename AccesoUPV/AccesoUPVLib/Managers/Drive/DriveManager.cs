using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    // Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    public class InvalidUserException : InvalidOperationException { }
    public class NoAvailableDriveException : IOException { }
    public class OpenedFilesException : IOException { }
    public abstract class DriveManager : ConnectionManager
    {
        public string ConnectedDrive { get; private set; }
        public string Drive { get; set; }
        public abstract string Address { get; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Domain { get; protected set; }
        public bool UseCredentials { get; set; }
        public bool SayYesToAll { get; set; }

        public override bool Connected
        {
            get { return ConnectedDrive != null; }
            protected set
            {
                if (value) ConnectedDrive = Drive;
                else ConnectedDrive = null;
            }
        }

        public DriveManager(string drive = null, string user = null, string password = null, string domain = null, bool useCredentials = false, bool sayYesToAll = false) : base()
        {
            Drive = drive;
            User = user;
            Password = password;
            Domain = domain;
            UseCredentials = useCredentials;
            SayYesToAll = sayYesToAll;

            conInfo.FileName = "net.exe";
            disInfo.FileName = "net.exe";
        }

        public static List<string> GetAvailableDrives()
        {
            List<string> drives = new List<string>();
            
            for (char letter = 'Z'; letter >= 'D'; letter--)
            {
                string d = letter + ":";
                if (!File.Exists(d)) drives.Add(d);
            }

            return drives;
        }

        protected override Process ConnectProcess()
        {
            if (Drive == null)
            {
                List<string> availableDrives = GetAvailableDrives();
                if (availableDrives.Count == 0) throw new NoAvailableDriveException();
                Drive = availableDrives[0];
            }
            conInfo.Arguments = $"use {Drive} {Address}";
            if (UseCredentials) conInfo.Arguments += $" {Password} /USER:{Domain}\\{User}";
            return Process.Start(conInfo);
        }

        protected override void ConnectionHandler(string output, string err)
        {
            // 55 - Error del sistema "El recurso no se encuentra disponible" (es decir, la dir no existe, por tanto, el usuario no es válido).
            if (output.Contains("55") || err.Contains("55"))
            {
                throw new InvalidUserException();
            }
        }

        protected override Process DisconnectProcess()
        {
            disInfo.Arguments = $"use {ConnectedDrive} /delete";
            if (SayYesToAll) disInfo.Arguments += "/y";
            return Process.Start(disInfo);
        }

        protected override void DisconnectionHandler(string output, string err)
        {
            //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
            if (output.Contains("/N)") || err.Contains("/N)")) {
                throw new OpenedFilesException();
            }
        }

    }
}
