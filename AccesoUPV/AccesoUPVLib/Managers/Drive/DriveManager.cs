using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    public abstract class DriveManager : ConnectionManager
    {
        public string ConnectedDrive { get; private set; }
        public string Drive { get; set; }
        public abstract string Address { get; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Domain { get; protected set; }
        public bool UseCredentials { get; set; }

        public override bool Connected
        {
            get { return ConnectedDrive != null; }
            protected set
            {
                if (value) ConnectedDrive = Drive;
                else ConnectedDrive = null;
            }
        }

        public DriveManager(string drive = null, string user = null, string password = null, string domain = null, bool useCredentials = false) : base()
        {
            Drive = drive;
            User = user;
            Password = password;
            Domain = domain;
            UseCredentials = useCredentials;

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

        public override bool Connect()
        {
            conInfo.Arguments = $"use {Drive ?? GetAvailableDrives()[0]} {Address}" + (UseCredentials ? $" {Password} /USER:{Domain}\\{User}" : "");
            Process proc = Process.Start(conInfo);
            return CheckProcess(proc, true);
        }

        public override bool Disconnect()
        {
            disInfo.Arguments = $"use {ConnectedDrive} /delete";
            Process proc = Process.Start(disInfo);
            return CheckProcess(proc, false);
        }

        public override Task<bool> ConnectAsync()
        {
            conInfo.Arguments = $"use {Drive ?? GetAvailableDrives()[0]} {Address}";
            Process proc = Process.Start(conInfo);

            return CheckProcessAsync(proc);
        }

        public override Task<bool> DisconnectAsync()
        {
            disInfo.Arguments = $"use {ConnectedDrive} /delete";
            Process proc = Process.Start(disInfo);

            return CheckProcessAsync(proc);
        }
    }
}
