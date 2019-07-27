﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    // Custom Exceptions
    // (Not having constructors defined creates an empty constructor automatically, and it calls its parent constructor as well)
    [Serializable]
    public class InvalidUserException : ArgumentException { }
    [Serializable]
    public class NotAvailableDriveException : IOException { }
    [Serializable]
    public class OpenedFilesException : IOException
    {
        public Action Continue { get; private set; }
        public Func<Task> ContinueAsync { get; private set; }

        public const string WarningTitle = "Archivos abiertos";
        public const string WarningMessage =
            "Existen archivos abiertos y/o búsquedas incompletas de directorios pendientes en el disco. Si no los cierra antes de desconectarse, podría perder datos.\n\n"
            + "¿Desea continuar la desconexión y forzar el cierre?";

        public OpenedFilesException(Action continueMethod, Func<Task> continueMethodAsync = null) : base()
        {
            Continue = continueMethod;
            ContinueAsync = continueMethodAsync;
        }
    }
    public abstract class NetworkDriveBase : ProcessConnector, INetworkDrive
    {
        public string ConnectedDrive { get; private set; }
        public string Drive { get; set; }
        public abstract string Address { get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseCredentials { get; set; }
        public bool YesToAll { get; set; }

        public override bool Connected
        {
            get => ConnectedDrive != null;
            protected set => ConnectedDrive = value ? Drive : null;
        }

        public virtual DriveDomain Domain { get; protected set; }

        protected NetworkDriveBase(string drive = null, string user = null, string password = null, bool useCredentials = false, bool yesToAll = false) : base()
        {
            Drive = drive;
            Username = user;
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
            if (string.IsNullOrEmpty(Username)) throw new ArgumentNullException(nameof(Username));

            conInfo.Arguments = $"use {Drive} {Address}";
            if (UseCredentials)
            {
                if (string.IsNullOrEmpty(Password)) throw new ArgumentNullException(nameof(Password));
                conInfo.Arguments += $" \"{Password}\" /USER:{Domain?.GetFullUsername(Username) ?? Username}";
            }
            if (YesToAll) conInfo.Arguments += " /y";
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
            return StartProcess(conInfo);
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
            if (YesToAll) disInfo.Arguments += " /y";
            return StartProcess(disInfo);
        }

        protected override void DisconnectionHandler(bool succeeded, string output, string error)
        {
            base.DisconnectionHandler(succeeded, output, error);

            if (!succeeded)
            {
                //Esa secuencia es parte de "(S/N)", con lo que deducimos que nos pide confirmación (porque tenemos archivos abiertos)
                if (output.Contains("/N)") || error.Contains("/N)"))
                {
                    throw new OpenedFilesException(ForceDisconnect, ForceDisconnectAsync) { };
                }
            }
        }

        private void ForceDisconnect()
        {
            bool oldYesToAll = this.YesToAll;
            this.YesToAll = true;
            this.Disconnect();
            this.YesToAll = oldYesToAll;
        }

        private async Task ForceDisconnectAsync()
        {
            bool oldYesToAll = this.YesToAll;
            this.YesToAll = true;
            await this.DisconnectAsync();
            this.YesToAll = oldYesToAll;
        }

        public void Open()
        {
            if (Connected) Process.Start(ConnectedDrive);
            else throw new InvalidOperationException("The drive must be connected in order to be opened");
        }
    }
}