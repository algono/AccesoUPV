using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.VPN
{
    public class VPNManager : VPNManagerBase
    {
        public override string Server { get; }
        public override string TestServer { get; }

        protected readonly IDictionary creationParams;

        public VPNManager(string server, string testServer = null, IDictionary creationParameters = null, string name = null) : base(name)
        {
            Server = server;
            TestServer = testServer ?? server;
            creationParams = creationParameters;
        }
        protected override PowerShell CreateShell()
        {
            PowerShell shell = base.CreateShell();
            if (creationParams != null) shell.AddParameters(creationParams);
            return shell;
        }
    }
}
