using Microsoft.VisualBasic;

namespace AccesoUPV.UnitTests
{
    static class SharedData
    {
        public static string Username { get; set; }
        public static string DSICDrivePass { get; set; }

        public static void PromptCredentials()
        {
            if (Username == null)
            {
                Username = Interaction.InputBox("Username:");
            }

            if (DSICDrivePass == null)
            {
                DSICDrivePass = Interaction.InputBox("Password (DSIC Drive):");
            }
        }
    }
}
