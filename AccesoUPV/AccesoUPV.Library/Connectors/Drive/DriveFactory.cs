using System.Collections.Generic;

namespace AccesoUPV.Library.Connectors.Drive
{
    public enum UPVDomain
    {
        Alumno, UPVNET
    }

    public static class DriveFactory
    {

        // DISCO W

        public static readonly IDictionary<UPVDomain, DriveDomain> UPVDomains = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.Alumno, new DriveDomain("alumno.upv.es", DomainStyle.AtSignStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("upvnet.upv.es", DomainStyle.AtSignStyle, "discos") }
        };

        private static readonly NetworkDriveConfig<UPVDomain> UPVConfig = new NetworkDriveConfig<UPVDomain>(GetAddressW, UPVDomains);

        public static NetworkDrive<UPVDomain> GetDriveW(string drive = null, string user = null, UPVDomain domain = UPVDomain.Alumno)
        {
            return new NetworkDrive<UPVDomain>(UPVConfig, domain, drive, user);
        }

        private static string GetAddressW(string username, DriveDomain domain) => $@"\\nasupv.upv.es\{domain.Folder}\{username[0]}\{username}";

        // DISCO DSIC

        public static readonly DriveDomain DSICDomain = new DriveDomain("DSIC");
        

        public static NetworkDrive GetDriveDSIC(string drive = null, string user = null, string password = null)
        {
            return new NetworkDrive(GetAddressDSIC, DSICDomain, drive, user, password);
        }

        private static string GetAddressDSIC(string username, DriveDomain domain) => $@"\\fileserver.dsic.upv.es\{username}";
    }
}
