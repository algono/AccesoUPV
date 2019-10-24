using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para Preferences.xaml
    /// </summary>
    public partial class Preferences
    {
        private bool _resetDone;

        public IAccesoUPVService Service { get; }

        public Preferences()
        {
            InitializeComponent();
        }

        public Preferences(IAccesoUPVService service)
        {
            Service = service;
            InitializeComponent();
            Load();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveChanges();
                this.Close();
            }
            catch (OperationCanceledException)
            {
                // The user canceled the process
            }
        }

        private void Load()
        {
            UserBox.Text = Service.User;

            VPNToUPVPrefs.Load();
            VPNToDSICPrefs.Load();

            DiscoWPrefs.Load();
            AsigDSICPrefs.Load();
            DiscoDSICPrefs.Load();

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (Service.Disco_W.Domain.Equals((UPVDomain)domain.Tag))
                {
                    domain.IsChecked = true;
                    break;
                }
            }

            NotifyIconCheckBox.IsChecked = Service.NotifyIcon;
        }

        private void SaveChanges()
        {
            string user = UserBox.Text;
            if (string.IsNullOrEmpty(user))
            {
                MessageBoxResult result = MessageBox.Show("No ha indicado ningún nombre de usuario.\n\n"
                + "Mientras no lo haga, no podrá acceder a ningún disco de red de la UPV.\n\n"
                + "¿Desea continuar?", "Ningún nombre de usuario", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result != MessageBoxResult.OK) throw new OperationCanceledException();
                user = null; // Avoids the user being an empty string (it causes problems)
            }

            Service.User = user;

            VPNToUPVPrefs.Save();
            VPNToDSICPrefs.Save();

            DiscoWPrefs.Save();
            AsigDSICPrefs.Save();
            DiscoDSICPrefs.Save();

            Service.Asig_DSIC.Password = Service.Disco_DSIC.Password;

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (domain.IsChecked ?? false)
                {
                    Service.Disco_W.Domain = (UPVDomain)domain.Tag;
                    break;
                }
            }

            Service.NotifyIcon = NotifyIconCheckBox.IsChecked ?? false;

            Service.SaveChanges();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Si continúa, se restablecerán los valores por defecto, y el programa se cerrará.\n\n"
            + "Al volverlo a abrir, será como si acabaras de ejecutar el programa por primera vez.\n\n"
            + "¿Desea continuar?", "Restablecer valores", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                Window mainWindow = Application.Current.MainWindow;
                mainWindow.Closed += MainWindow_Closed; // If the window is closed, clear the settings
                mainWindow.Close(); // Closes the main window to trigger shutdown (if needed)
                if (!_resetDone) mainWindow.Closed -= MainWindow_Closed; // If it was somehow canceled, dont do it anymore
            }
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            Service.ClearSettings();
            _resetDone = true;
        }

    }
}
