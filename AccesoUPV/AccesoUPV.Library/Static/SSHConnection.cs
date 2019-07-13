using System;

namespace AccesoUPV.Library.Static
{
    public static class SSHConnection
    {
        public static string
            DISCA_SSH = "home-labs.disca.upv.es", KAHAN_SSH = "kahan.dsic.upv.es";

        public static void ConnectToDISCA(string username, string password) => ConnectTo(DISCA_SSH, username, password);
        public static void ConnectToKahan(string username, string password) => ConnectTo(KAHAN_SSH, username, password);

        public static void ConnectTo(string host, string username, string password)
        {
            throw new NotImplementedException();
        }

    }
}