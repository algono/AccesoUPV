using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public class VPN
    {
        public static int PING_TIMEOUT = 500, FINAL_PING_TIMEOUT = 4000;
        public string Name { get; set; }
        public string Server { get; }

        public VPN(string name, string server)
        {
            Name = name;
            Server = server;
        }

        public bool isReachable()
        {
            Process p = Process.Start("ping.exe", $"-n 1 -w {PING_TIMEOUT} {Server}");
            p.WaitForExit();
            return p.ExitCode == 0;
        }

    }
}
