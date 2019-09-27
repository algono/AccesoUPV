using System.Collections.Generic;
using System.IO;

namespace AccesoUPV.Library.Connectors.Drive
{
    #region Clases UPV
    public enum UPVDomain
    {
        Alumno, UPVNET
    }

    /**
    * 1223 - Error del sistema "El usuario o la contraseña son incorrectos".
    * Esto es un bug porque debería obtener las credenciales de la conexión con la UPV
    * (ya sea VPN o WiFi).
    * Por ahora el bug sólo se ha visto desde red WiFi, y la solución es reconectarse.
    */
    public class CredentialsBugException : IOException
    {
        public const string ERROR_BUG = "Ha habido un error de credenciales.\n\n"
            + "Se comenzará ahora el proceso de reconexión.";
        public CredentialsBugException() : base(ERROR_BUG) { }
    }
    #endregion

    public static class DriveFactory
    {

        #region Disco W
        public static readonly IDictionary<UPVDomain, DriveDomain> UPVDomains = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.Alumno, new DriveDomain("alumno.upv.es", DomainStyle.AtSignStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("upvnet.upv.es", DomainStyle.AtSignStyle, "discos") }
        };

        public static NetworkDrive<UPVDomain> GetDriveW(char drive = default, string user = null, UPVDomain domain = UPVDomain.Alumno)
        {
            NetworkDrive<UPVDomain> driveW = new NetworkDrive<UPVDomain>(GetAddressW, UPVDomains, drive, user)
            {
                Domain = domain,
                Name = "Disco W"
            };

            driveW.ProcessConnected += DriveW_ProcessConnected;

            return driveW;
        }

        private static void DriveW_ProcessConnected(object sender, ProcessEventArgs e)
        {
            if (!e.Succeeded && e.OutputOrErrorContains("1223"))
            {
                throw new CredentialsBugException();
            }
        }

        private static string GetAddressW(string username, DriveDomain domain) => $@"\\nasupv.upv.es\{domain.Folder}\{username[0]}\{username}";
        #endregion

        #region Disco DSIC
        public static readonly DriveDomain DSICDomain = new DriveDomain("DSIC");

        public static NetworkDrive GetDriveDSIC(char drive = default, string user = null, string password = null)
            => new NetworkDrive(GetAddressDSIC, drive, DSICDomain, user, password) { Name = "Disco DSIC" };

        private static string GetAddressDSIC(string username, DriveDomain domain) => $@"\\fileserver.dsic.upv.es\{username}";
        #endregion
    }
}
