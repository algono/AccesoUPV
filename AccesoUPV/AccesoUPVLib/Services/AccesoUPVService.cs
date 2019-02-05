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
        public VPNManager UPV_VPN { get; }
        public VPNManager DSIC_VPN { get; }
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

        public AccesoUPVService()
        {
            user = Settings.Default.User;

            UPV_VPN = new VPNManager(Servers.VPN_UPV, Servers.WEB_UPV, GetUPVCreationParameters(), Settings.Default.VPN_UPVName);
            DSIC_VPN = new VPNManager(Servers.VPN_DSIC, Servers.PORTAL_DSIC, GetDSICCreationParameters(), Settings.Default.VPN_DSICName);

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

        protected static IDictionary GetUPVCreationParameters()
        {
            IDictionary creationParameters = new Dictionary<string, object>();

            creationParameters.Add("AuthenticationMethod", "Eap");
            creationParameters.Add("EncryptionLevel", "Required");
            creationParameters.Add("TunnelType", "Sstp");

            System.Xml.XmlDocument ConfigXml = new System.Xml.XmlDocument();
            ConfigXml.Load("Resources/UPV_Config.xml");
            creationParameters.Add("EapConfigXmlStream", ConfigXml);

            return creationParameters;
        }

        protected static IDictionary GetDSICCreationParameters()
        {
            IDictionary creationParameters = new Dictionary<string, object>();

            creationParameters.Add("AuthenticationMethod", "MSChapv2");
            creationParameters.Add("EncryptionLevel", "Optional");
            creationParameters.Add("L2tpPsk", "dsic");
            creationParameters.Add("TunnelType", "L2tp");
            creationParameters.Add("Force", true);

            return creationParameters;
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
