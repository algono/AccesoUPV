using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para Preferences.xaml
    /// </summary>
    public partial class Preferences
    {
        private readonly IAccesoUPVService _service;

        public Preferences()
        {
            InitializeComponent();
        }

        public Preferences(IAccesoUPVService service) : this()
        {
            _service = service;
            Load();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            this.Close();
        }

        private void Load()
        {
            UserBox.Text = _service.User;

            VPNToUPVBox.Text = _service.VPN_UPV.Name;
            VPNToDSICBox.Text = _service.VPN_DSIC.Name;

            // TODO: Refactor VPN and Drive preferences to generic User Controls
            LoadDrives();
            ShowBusyDrives.Click += (s, e) =>
            {
                if (ShowBusyDrives.IsChecked ?? false)
                {
                    LoadDrives();
                }
                else
                {
                    WDriveBox.ItemsSource
                    = NetworkDrive.SelectAvailable(
                        WDriveBox.ItemsSource
                        as IEnumerable<string>);
                }
            };

            string WDriveLetter = _service.Disco_W.Drive;
            if (!string.IsNullOrEmpty(WDriveLetter))
            {
                WDriveCheckBox.IsChecked = true;
                WDriveBox.SelectedItem = WDriveLetter;
            }

            string DSICDriveLetter = _service.Disco_DSIC.Drive;
            if (!string.IsNullOrEmpty(DSICDriveLetter))
            {
                DSICDriveCheckBox.IsChecked = true;
                DSICDriveBox.SelectedItem = DSICDriveLetter;
            }

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (_service.Disco_W.Domain.Equals((UPVDomain)domain.Tag))
                {
                    domain.IsChecked = true;
                    break;
                }
            }

            PassDSICBox.Text = _service.Disco_DSIC.Password;
            SavePassCheckBox.IsChecked = _service.SavePasswords;

        }

        private void LoadDrives()
        {
            bool onlyIfAvailable = !(ShowBusyDrives.IsChecked ?? false);
            List<string> availableDrives = NetworkDrive.GetDrives(onlyIfAvailable);
            WDriveBox.ItemsSource = availableDrives;
            DSICDriveBox.ItemsSource = availableDrives;
        }

        private void SaveChanges()
        {
            _service.User = UserBox.Text;

            _service.VPN_UPV.Name = VPNToUPVBox.Text;
            _service.VPN_DSIC.Name = VPNToDSICBox.Text;

            _service.Disco_W.Drive =
                (WDriveCheckBox.IsChecked ?? false)
                ? WDriveBox.SelectedItem.ToString()
                : null;

            _service.Disco_DSIC.Drive =
                (DSICDriveCheckBox.IsChecked ?? false)
                ? DSICDriveBox.SelectedItem.ToString()
                : null;

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (domain.IsChecked ?? false)
                {
                    _service.Disco_W.Domain = (UPVDomain)domain.Tag;
                    break;
                }
            }

            _service.Disco_DSIC.Password = PassDSICBox.Text;
            _service.SavePasswords = SavePassCheckBox.IsChecked ?? false;

            _service.SaveChanges();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Si continúa, se restablecerán los valores por defecto, y el programa se cerrará.\n\n"
            + "Al volverlo a abrir, será como si acabaras de ejecutar el programa por primera vez.\n\n"
            + "¿Desea continuar?", "Restablecer valores", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                _service.ClearSettings();
                Application.Current.Shutdown();
            }
        }
    }
}
