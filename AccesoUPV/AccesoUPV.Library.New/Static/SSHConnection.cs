using System.Diagnostics;

namespace AccesoUPV.Library.Static
{
    public static partial class SSHConnection
    {
        public const string
            DISCA_SSH = "home-labs.disca.upv.es", KAHAN_SSH = "kahan.dsic.upv.es";

        public static void ConnectToDisca() => ConnectTo(DISCA_SSH);
        public static void ConnectToDisca(string user) => ConnectTo(DISCA_SSH, user);
        public static void ConnectToKahan() => ConnectTo(KAHAN_SSH);
        public static void ConnectToKahan(string user) => ConnectTo(KAHAN_SSH, user);

        public static void ConnectTo(string server) => Process.Start(SSH_PATH, server);
        public static void ConnectTo(string server, string user) => Process.Start(SSH_PATH, $"{user}@{server}");
    }
}