using System;
using System.Collections.Generic;
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
    public class NetworkDriveUPV : NetworkDrive
    {
        public static readonly IDictionary<UPVDomain, DriveDomain> DomainsUPV = new Dictionary<UPVDomain, DriveDomain>()
        {
            { UPVDomain.Alumno, new DriveDomain("alumno.upv.es", DomainStyle.AtSignStyle, "alumnos") },
            { UPVDomain.UPVNET, new DriveDomain("upvnet.upv.es", DomainStyle.AtSignStyle, "discos") }
        };

        private UPVDomain domainUPV;

        public new UPVDomain Domain
        {
            get => domainUPV;
            set
            {
                domainUPV = value;
                base.Domain = DomainsUPV[value];
            }
        }

        public NetworkDriveUPV(Func<string, DriveDomain, string> addressGetter, UPVDomain domain = UPVDomain.Alumno, string drive = null, string user = null) : base(addressGetter, null, drive, user)
        {
            Domain = domain;
        }

        protected override void OnProcessConnected(ProcessEventArgs e)
        {
            base.OnProcessConnected(e);
            if (!e.Succeeded)
            {
                if (e.Output.Contains("1223") || e.Error.Contains("1223"))
                {
                    throw new CredentialsBugException();
                }
            }
        }
    }
}
