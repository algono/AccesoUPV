using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    /**
    * 1223 - Error del sistema "El usuario o la contraseña son incorrectos".
    * Esto es un bug porque debería obtener las credenciales de la conexión con la UPV
    * (ya sea VPN o WiFi).
    * Por ahora el bug sólo se ha visto desde red WiFi, y la solución es reconectarse.
    */
    public class CredentialsBugException : IOException { }

    public enum UPVDomain
    {
        Alumno, UPVNET
    }
    public class WDriveManager : DriveManager
    {
        public override string Address
        {
            get
            {
                return $"\\\\nasupv.upv.es\\{base.Domain}\\{User[0]}\\{User}";
            }
        }
        protected UPVDomain _Domain;
        public new UPVDomain Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
                base.Domain = (value == UPVDomain.Alumno ? "alumnos" : "discos");
            }
        }

        public WDriveManager(string user = null, char? drive = null, UPVDomain domain = UPVDomain.Alumno) : base(drive, user)
        {
            Domain = domain;
        }

        protected override void ConnectionHandler(bool succeeded, string output, string err)
        {
            base.ConnectionHandler(succeeded, output, err);
            if (!succeeded)
            {
                if (output.Contains("1223") || err.Contains("1223"))
                {
                    throw new CredentialsBugException();
                }
            }
        }
    }
}
