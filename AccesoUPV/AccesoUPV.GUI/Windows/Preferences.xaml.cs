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
            SaveChanges();
            this.Close();
        }

        private void Load()
        {
            UserBox.Text = Service.User;

            VPNToUPVPrefs.Load();
            VPNToDSICPrefs.Load();

            DiscoWPrefs.Load();
            DiscoDSICPrefs.Load();

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (Service.Disco_W.Domain.Equals((UPVDomain)domain.Tag))
                {
                    domain.IsChecked = true;
                    break;
                }
            }

        }

        private void SaveChanges()
        {
            Service.User = UserBox.Text;

            VPNToUPVPrefs.Save();
            VPNToDSICPrefs.Save();

            DiscoWPrefs.Save();
            DiscoDSICPrefs.Save();

            foreach (RadioButton domain in DomainsUPV.Children)
            {
                if (domain.IsChecked ?? false)
                {
                    Service.Disco_W.Domain = (UPVDomain)domain.Tag;
                    break;
                }
            }

            Service.SaveChanges();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Si continúa, se restablecerán los valores por defecto, y el programa se cerrará.\n\n"
            + "Al volverlo a abrir, será como si acabaras de ejecutar el programa por primera vez.\n\n"
            + "¿Desea continuar?", "Restablecer valores", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                Service.ClearSettings();
                Application.Current.Shutdown();
            }
        }
    }
}
