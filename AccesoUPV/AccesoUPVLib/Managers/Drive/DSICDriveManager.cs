using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    class InvalidCredentials : InvalidOperationException { }
    public class DSICDriveManager : DriveManager
    {
        public override string Address
        {
            get
            {
                return $"\\\\fileserver.dsic.upv.es\\{User}";
            }
        }

        public DSICDriveManager(string drive = null, string user = null, string password = null) : base(drive, user, password, "DSIC", true)
        {
        }

        protected override void ConnectionHandler(string output, string err)
        {
            base.ConnectionHandler(output, err);

            // 86 - Error del sistema "La contraseña de red es incorrecta"
            // 1326 - Error del sistema "El usuario o la contraseña son incorrectos"
            //Cuando las credenciales son erróneas, da uno de estos dos errores de forma arbitraria.
            if (output.Contains("86") || err.Contains("86") || output.Contains("1326") || err.Contains("1326")) {
                throw new InvalidCredentials();
            }
        }
    }
}
