using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    public static class DriveFactory
    {

        // DISCO W

        public static NetworkDriveUPV GetDriveW(string drive = null, string user = null, UPVDomain domain = UPVDomain.Alumno)
        {
            return new NetworkDriveUPV(GetAddressW, domain, drive, user);
        }

        private static string GetAddressW(string Username, DriveDomain domain) => $@"\\nasupv.upv.es\{domain.Folder}\{Username[0]}\{Username}";

        // DISCO DSIC

        public static readonly DriveDomain domainDSIC = new DriveDomain("DSIC");
        public static NetworkDrive GetDriveDSIC(string drive = null, string user = null, string password = null)
        {
            return new NetworkDrive(GetAddressDSIC, domainDSIC, drive, user, password);
        }

        private static string GetAddressDSIC(string Username, DriveDomain domain) => $@"\\fileserver.dsic.upv.es\{Username}";
    }
}
