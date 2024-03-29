﻿using AccesoUPV.Library.Connectors.VPN;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            + "Debe reestablecer su conexión a la UPV manualmente (VPN o WiFi).";
        public CredentialsBugException() : base(ERROR_BUG) { }
    }
    #endregion

    public static class DriveFactory
    {

        #region Disco W
        public static readonly IDictionary<UPVDomain, DriveDomain> UPVDomains = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.Alumno, new DriveDomain("ALUMNO", DomainStyle.BackSlashStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("UPVNET", DomainStyle.BackSlashStyle, "discos") }
        };

        public static NetworkDrive<UPVDomain> GetDriveW(char drive = default, string user = null, UPVDomain domain = UPVDomain.Alumno)
        {
            bool connectedViaEthernet = VPNFactory.UPVTest.GetValidInterfaces().FirstOrDefault()?.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet;

            NetworkDrive<UPVDomain> driveW = new NetworkDrive<UPVDomain>(GetAddressW, UPVDomains, drive, user)
            {
                Domain = domain,
                Name = "Disco W",
                DefaultLetter = 'W',
                NeedsUsername = true,
                ExplicitUserArgument = connectedViaEthernet,
                NeedsPassword = connectedViaEthernet
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

        public const string WAddress = "nasupv.upv.es";

        private static string GetAddressW(string username, DriveDomain domain) => $@"\\{WAddress}\{domain.Folder}\{username[0]}\{username}";
        #endregion

        #region Disco DSIC
        public const string DSICDrivesAddress = @"fileserver.dsic.upv.es";
        public static readonly string DSICAsigAddress = $@"\\{DSICDrivesAddress}\asig";

        public static readonly DriveDomain DSICDomain = new DriveDomain("DSIC");

        public static NetworkDrive GetAsigDriveDSIC(char drive = default, string user = null, bool isPasswordStored = false)
            => new NetworkDrive(DSICAsigAddress, drive, DSICDomain, user) { Name = "Asig DSIC", ExplicitUserArgument = true, NeedsPassword = true, AreCredentialsStored = isPasswordStored };

        public static NetworkDrive GetDriveDSIC(char drive = default, string user = null, bool isPasswordStored = false)
            => new NetworkDrive(GetAddressDSIC, drive, DSICDomain, user) { Name = "Disco DSIC", ExplicitUserArgument = true, NeedsPassword = true, AreCredentialsStored = isPasswordStored };

        private static string GetAddressDSIC(string username, DriveDomain domain) => $@"\\{DSICDrivesAddress}\{username}";
        #endregion
    }
}
