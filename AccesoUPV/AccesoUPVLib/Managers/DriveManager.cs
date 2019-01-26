using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers
{
    public class DriveManager : ConnectionManager<bool>
    {
        public string ConnectedDrive { get; private set; }
        public string User { get; set; }
        public string Drive { get; set; }
        public string Address { get; }

        public override bool Connected
        {
            get { return ConnectedDrive != null; }
            protected set
            {
                if (value) ConnectedDrive = Drive;
                else ConnectedDrive = null;
            }
        }

        public DriveManager(string address, string user = null, string drive = null) : base()
        {
            Address = address;
            User = user;
            Drive = drive;

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
            conInfo.Arguments = $"use {Drive ?? GetAvailableDrives()[0]} {Address}";
            Process proc = Process.Start(conInfo);
            CheckProcess(proc, true);
            return Connected;
        }

        public override bool Disconnect()
        {
            disInfo.Arguments = $"use {ConnectedDrive} /delete";
            Process proc = Process.Start(disInfo);
            CheckProcess(proc, false);
            return Connected;
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
