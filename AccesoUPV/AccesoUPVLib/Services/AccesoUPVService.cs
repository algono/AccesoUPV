using AccesoUPV.Lib.Managers;
using AccesoUPV.Lib.Managers.Drive;
using AccesoUPV.Lib.Managers.VPN;
using AccesoUPV.Lib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AccesoUPV.Lib.Services
{
    public class AccesoUPVService
    {
        //Managers
        public VPNManager VPN_UPV { get; }
        public VPNManager VPN_DSIC { get; }
        public WDriveManager WDrive { get; }
        public DSICDriveManager DSICDrive { get; }

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

        protected static IDictionary UPVCreationParameters { get; }
        protected static IDictionary DSICCreationParameters { get; }

        static AccesoUPVService()
        {
            //UPV creation parameters
            UPVCreationParameters = new Dictionary<string, object>();

            UPVCreationParameters.Add("AuthenticationMethod", "Eap");
            UPVCreationParameters.Add("EncryptionLevel", "Required");
            UPVCreationParameters.Add("TunnelType", "Sstp");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            UPVCreationParameters.Add("EapConfigXmlStream", ConfigXml);

            //DSIC creation parameters
            DSICCreationParameters = new Dictionary<string, object>();

            DSICCreationParameters.Add("AuthenticationMethod", "MSChapv2");
            DSICCreationParameters.Add("EncryptionLevel", "Optional");
            DSICCreationParameters.Add("L2tpPsk", "dsic");
            DSICCreationParameters.Add("TunnelType", "L2tp");
            DSICCreationParameters.Add("Force", true);
        }

        public AccesoUPVService()
        {
            user = Settings.Default.User;

            VPN_UPV = new VPNManager(Servers.VPN_UPV, Servers.WEB_UPV, UPVCreationParameters, Settings.Default.VPN_UPVName);
            VPN_DSIC = new VPNManager(Servers.VPN_DSIC, Servers.PORTAL_DSIC, DSICCreationParameters, Settings.Default.VPN_DSICName);

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

            Settings.Default.VPN_UPVName = VPN_UPV.Name;
            Settings.Default.VPN_DSICName = VPN_DSIC.Name;

            Settings.Default.WDriveLetter = WDrive.Drive.GetValueOrDefault();
            Settings.Default.WDriveDomain = WDrive.Domain.ToString();

            Settings.Default.DSICDriveLetter = DSICDrive.Drive.GetValueOrDefault();
            Settings.Default.DSICPassword = SavePasswords ? DSICDrive.Password : null;

            Settings.Default.Save();
        }

        public static void ConnectToLinuxDesktop()
        {
            ConnectToRemoteDesktop(Servers.LINUX_DSIC);
        }
        public static void ConnectToWindowsDesktop()
        {
            ConnectToRemoteDesktop(Servers.WIN_DSIC);
        }
        protected static void ConnectToRemoteDesktop(string server)
        {
            Process.Start("mstsc.exe", $"/v:{server}").WaitAndCheck();
        }

    }
}
