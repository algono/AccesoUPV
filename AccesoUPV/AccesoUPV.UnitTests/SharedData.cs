using AccesoUPV.Library.Connectors.VPN;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AccesoUPV.UnitTests
{
    [TestClass]
    public static class SharedData
    {
        public static TestContext TestContext { get; set; }

        public static string Username { get; set; }
        public static string DSICDrivePass { get; set; }

        public static List<VPN> VPNs { get; } = new List<VPN>();

        [AssemblyInitialize]
        public static void PromptCredentials(TestContext context)
        {
            TestContext = context;
            Username = Interaction.InputBox("Username:");
            DSICDrivePass = Interaction.InputBox("Password (DSIC Drive):");
        }
    }
}
