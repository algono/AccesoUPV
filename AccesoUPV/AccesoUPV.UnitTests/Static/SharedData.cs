using AccesoUPV.Library.Connectors.VPN;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AccesoUPV.UnitTests
{
    public static class SharedData
    {
        public static string Username { get; set; }
        public static string DSICDrivePass { get; set; }

        public static List<VPN> VPNs { get; } = new List<VPN>();

        public static void PromptUsername()
        {
            if (Username == default)
            {
                Username = Interaction.InputBox("Username:");
            }
        }

        public static void PromptPasswordDSIC()
        {
            if (DSICDrivePass == default)
            {
                DSICDrivePass = Interaction.InputBox("Password (DSIC Drive):");
            }
        }
    }
}
