using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System.Windows;

namespace AccesoUPV.GUI
{
    /// <summary>
    /// Lógica de interacción para Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        private readonly IAccesoUPVService _service;

        public Preferences()
        {
            InitializeComponent();
        }

        public Preferences(IAccesoUPVService service) : this()
        {
            _service = service;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
            this.Close();
        }

        private void SaveChanges()
        {
            _service.User = UserBox.Text;

            _service.VPN_UPV.Name = VPNToUPVBox.Text;
            _service.VPN_DSIC.Name = VPNToDSICBox.Text;

            if (DriveWCheckBox.IsChecked ?? false)
            {
                _service.WDrive.Drive = WDriveBox.SelectedItem.ToString();
            }
            else
            {
                _service.WDrive.Drive = null;
            }

            if (DriveDSICCheckBox.IsChecked ?? false)
            {
                _service.DSICDrive.Drive = DSICDriveBox.SelectedItem.ToString();
            }
            else
            {
                _service.DSICDrive.Drive = null;
            }
            

            if (DominioAlumnoRadio.IsChecked ?? false)
            {
                _service.WDrive.Domain = UPVDomain.Alumno;
            }
            else
            {
                _service.WDrive.Domain = UPVDomain.UPVNET;
            }

            _service.DSICDrive.Password = PassDSICBox.Text;
            _service.SavePasswords = SavePassCheckBox.IsChecked ?? false;

            _service.SaveChanges();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Si continúa, se reestablecerán los valores por defecto, y el programa se cerrará.\n\n"
            + "Al volverlo a abrir, será como si acabaras de ejecutar el programa por primera vez.\n\n"
            + "¿Desea continuar?", "Reestablecer valores", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                _service.ClearSettings();
                Application.Current.Shutdown();
            }
        }
    }
}
