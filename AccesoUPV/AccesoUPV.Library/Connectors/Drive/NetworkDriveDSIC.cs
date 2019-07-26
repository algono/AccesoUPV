using System;

namespace AccesoUPV.Library.Connectors.Drive
{
    [Serializable]
    public class InvalidCredentials : ArgumentException { }
    public class NetworkDriveDSIC : NetworkDriveBase
    {
        public override string Address => $@"\\fileserver.dsic.upv.es\{Username}";
        private static readonly DriveDomain DSICDomain = new DriveDomain("DSIC");
        public override DriveDomain Domain
        {
            get => DSICDomain;
            protected set => throw new InvalidOperationException("The domain for this class can't be changed.");
        }

        public NetworkDriveDSIC(string user = null, string password = null, string drive = null) : base(drive, user, password, true)
        {
        }

        protected override void ConnectionHandler(bool succeeded, string output, string error)
        {
            base.ConnectionHandler(succeeded, output, error);

            if (!succeeded)
            {
                // 86 - Error del sistema "La contraseña de red es incorrecta"
                // 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
                //Cuando las credenciales son erróneas, da uno de estos dos errores de forma arbitraria.
                if (output.Contains("86") || error.Contains("86") || output.Contains("1326") || error.Contains("1326"))
                {
                    throw new InvalidCredentials();
                }
            }
        }
    }
}
