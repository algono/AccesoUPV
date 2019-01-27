using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib.Managers.Drive
{
    public class WDriveManager : DriveManager
    {
        public enum UPVDomain
        {
            Alumno, UPVNET
        }
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

        public WDriveManager(string drive = null, string user = null, UPVDomain domain = UPVDomain.Alumno) : base(drive, user)
        {
            Domain = domain;
        }
    }
}
