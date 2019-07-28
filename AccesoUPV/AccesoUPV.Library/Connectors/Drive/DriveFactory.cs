using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public static class DriveFactory
    {

        // DISCO W

        public static readonly DriveDomain domainAlumno = new DriveDomain("alumno.upv.es", DomainStyle.AtSignStyle, "alumnos"),
                                            domainUPVNET = new DriveDomain("upvnet.upv.es", DomainStyle.AtSignStyle, "discos");

        public static NetworkDrive GetDriveW(string drive = null, string user = null, UPVDomain domain = UPVDomain.Alumno)
        {
            var WDrive = new NetworkDrive(GetAddressW, GetDriveDomain(domain), drive, user);
            WDrive.ProcessConnected += OnConnectW;
            return WDrive;
        }

        private static string GetAddressW(string Username, DriveDomain domain) => $@"\\nasupv.upv.es\{domain.Folder}\{Username[0]}\{Username}";

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

        private static void OnConnectW(object sender, ProcessEventArgs e)
        {
            if (!e.Succeeded)
            {
                if (e.Output.Contains("1223") || e.Error.Contains("1223"))
                {
                    throw new CredentialsBugException();
                }
            }
        }

        // DISCO DSIC

        public static readonly DriveDomain domainDSIC = new DriveDomain("DSIC");
        public static NetworkDrive GetDriveDSIC(string drive = null, string user = null, string password = null)
        {
            return new NetworkDrive(GetAddressDSIC, domainDSIC, drive, user, password);
        }

        private static string GetAddressDSIC(string Username, DriveDomain domain) => $@"\\fileserver.dsic.upv.es\{Username}";
    }
}
