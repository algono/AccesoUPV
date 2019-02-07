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

    public static class UPVDomain
    {
        public const string Alumno = "alumnos", UPVNET = "discos";
    }
    public class WDriveManager : DriveManagerBase
    {
        public override string Address
        {
            get
            {
                return $"\\\\nasupv.upv.es\\{Domain}\\{User[0]}\\{User}";
            }
        }

        public override string Domain
        {
            get
            {
                return base.Domain;
            }
            set
            {
                if (value != UPVDomain.Alumno && value != UPVDomain.UPVNET)
                {
                    throw new ArgumentOutOfRangeException("The only domains permitted here are the ones within the static class UPVDomain");
                }
                base.Domain = value;
            }
        }

        public WDriveManager(string user = null, string drive = null, string domain = UPVDomain.Alumno) : base(drive, domain, user)
        {
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
