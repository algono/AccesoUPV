using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using AccesoUPV.Lib.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Lib
{
    public class AccesoUPVService
    {
        //Managers
        private VPNManager UPV_VPN, DSIC_VPN;
        private DriveManager WDrive, DSICDrive;

        //Servers
        public static string LINUX_DSIC = "linuxdesktop.dsic.upv.es";
        public static string WIN_DSIC = "windesktop.dsic.upv.es";
        public static string DISCA_SSH = "home-labs.disca.upv.es", KAHAN_SSH = "kahan.dsic.upv.es";

        private string user;

        public string User
        {
            get { return user; }
            set
            {
                user = value;
                WDrive.User = value;
                DSICDrive.User = value;
            }
        }


        public AccesoUPVService()
        {
            User = Settings.Default.User;
            UPV_VPN = UPVManager.Create();
            DSIC_VPN = DSICManager.Create();
            WDrive = new WDriveManager(Settings.Default.WDriveLetter + ":", User, GetSetting_WDriveDomain());
            DSICDrive = new DSICDriveManager();
        }

        protected UPVDomain GetSetting_WDriveDomain()
        {
            UPVDomain result = UPVDomain.Alumno;  //make compiler happy, set an initial value
            Array domains = Enum.GetValues(typeof(UPVDomain));
            foreach (UPVDomain domain in domains)
            {
                if (domain.ToString() == Settings.Default.WDriveDomain)
                {
                    result = domain;
                    break;
                }
            }
            return result;
        }

        public void SaveChanges()
        {
            Settings.Default.User = User;

            Settings.Default.VPN_UPVName = UPV_VPN.Name;
            Settings.Default.VPN_DSICName = DSIC_VPN.Name;

            Settings.Default.WDriveLetter = WDrive.Drive[0];
            Settings.Default.WDriveDomain = WDrive.Domain.ToString();

            Settings.Default.DSICDriveLetter = DSICDrive.Drive[0];

            Settings.Default.Save();
        }

        public Task<bool> ConnectToUPV()
        {
            return UPV_VPN.ConnectAsync();
        }

    }
}
