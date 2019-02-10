using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    class InvalidCredentials : ArgumentException { }
    public class DSICDriveManager : DriveManagerBase
    {
        public override string Address
        {
            get
            {
                return $"\\\\fileserver.dsic.upv.es\\{UserName}";
            }
        }
        private static readonly DriveDomain DSICDomain = new DriveDomain("DSIC");
        public override DriveDomain Domain
        {
            get
            {
                return DSICDomain;
            }

            protected set
            {
                throw new InvalidOperationException("The domain for this class can't be changed.");
            }
        }
        
        public DSICDriveManager(string user = null, string password = null, string drive = null) : base(drive, user, password, true)
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
