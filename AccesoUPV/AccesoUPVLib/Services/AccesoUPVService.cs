using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using AccesoUPV.Lib.Properties;
using System;
using System.Diagnostics;

namespace AccesoUPV.Lib.Services
{
    public class AccesoUPVService
    {
        //Managers
        private VPNManager UPV_VPN, DSIC_VPN;
        private WDriveManager WDrive;
        private DSICDriveManager DSICDrive;

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
        public bool SavePasswords { get; set; }

        public AccesoUPVService()
        {
            user = Settings.Default.User;

            UPV_VPN = UPVManager.Create();
            DSIC_VPN = DSICManager.Create();
            
            WDrive = new WDriveManager(User, GetCharSetting("WDriveLetter"), GetSetting_WDriveDomain());
            
            DSICDrive = new DSICDriveManager(User);

            DSICDrive.Password = Settings.Default.DSICPassword;
            SavePasswords = DSICDrive.Password != null;

            DSICDrive.Drive = GetCharSetting("DSICDriveLetter");
        }

        protected static char? GetCharSetting(string propertyName)
        {
            try
            {
                return ((char) Settings.Default[propertyName]);
            }
            catch (NullReferenceException)
            {
                //Esto ocurre cuando la propiedad (de tipo char) no está inicializada.
                //Como el tipo char es primitivo, no puede ser null, y por eso lanza una excepción.
                return null;
            }
        }

        protected static UPVDomain GetSetting_WDriveDomain()
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

            Settings.Default.WDriveLetter = WDrive.Drive.GetValueOrDefault();
            Settings.Default.WDriveDomain = WDrive.Domain.ToString();

            Settings.Default.DSICDriveLetter = DSICDrive.Drive.GetValueOrDefault();
            Settings.Default.DSICPassword = SavePasswords ? DSICDrive.Password : null;

            Settings.Default.Save();
        }

        public void ConnectToLinuxDesktop()
        {
            ConnectToRemoteDesktop(LINUX_DSIC);
        }
        public void ConnectToWindowsDesktop()
        {
            ConnectToRemoteDesktop(WIN_DSIC);
        }
        protected void ConnectToRemoteDesktop(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

    }
}
