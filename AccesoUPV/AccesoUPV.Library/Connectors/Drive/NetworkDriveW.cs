using System;
using System.IO;

namespace AccesoUPV.Library.Connectors.Drive
{
    /**
    * 1223 - Error del sistema "El usuario o la contraseña son incorrectos".
    * Esto es un bug porque debería obtener las credenciales de la conexión con la UPV
    * (ya sea VPN o WiFi).
    * Por ahora el bug sólo se ha visto desde red WiFi, y la solución es reconectarse.
    */
    [Serializable]
    public class CredentialsBugException : IOException { }

    public enum UPVDomain
    {
        Alumno, UPVNET
    }
    public class NetworkDriveW : NetworkDriveBase
    {
        public override string Address => $@"\\nasupv.upv.es\{base.Domain.Folder}\{UserName[0]}\{UserName}";

        public static readonly DriveDomain domainAlumno = new DriveDomain("alumno.upv.es", DomainStyle.AtSignStyle, "alumnos"),
                                            domainUPVNET = new DriveDomain("upvnet.upv.es", DomainStyle.AtSignStyle, "discos");

        public static DriveDomain GetDriveDomain(UPVDomain domain)
        {
            switch (domain)
            {
                case UPVDomain.Alumno:
                    return domainAlumno;
                case UPVDomain.UPVNET:
                    return domainUPVNET;
                default:
                    throw new ArgumentOutOfRangeException("Argument not valid.");
            }
        }

        private UPVDomain wDomain;

        public new UPVDomain Domain
        {
            get => wDomain;
            set
            {
                wDomain = value;
                base.Domain = GetDriveDomain(value);
            }
        }

        public NetworkDriveW(string user = null, string drive = null, UPVDomain domain = UPVDomain.Alumno) : base(drive, user)
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
