using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AccesoUPV.GUI.Windows.UserControls;
using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;

namespace AccesoUPV.GUI.Windows.MainPages
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public IAccesoUPVService Service { get; }

        public Main()
        {
            InitializeComponent();
        }

        public Main(IAccesoUPVService service)
        {
            Service = service;
            InitializeComponent();
        }

        private async Task ConnectWDrive(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.WDrive.Username)))
            {
                Drive_UsernameWasNull();
            }
        }

        private async Task DisconnectWDrive(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (OpenedFilesException ex)
            {
                await Drive_OpenedFiles(ex);
            }
        }

        private async Task ConnectDSICDrive(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.DSICDrive.Username)))
            {
                Drive_UsernameWasNull();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.DSICDrive.Password)))
            {
                MessageBox.Show("No ha indicado ninguna contraseña. Indíquela en los ajustes.", "Falta contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
                new Preferences(Service).ShowDialog();
            }
        }

        private async Task DisconnectDSICDrive(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (OpenedFilesException ex)
            {
                await Drive_OpenedFiles(ex);
            }
        }

        private static async Task Drive_OpenedFiles(OpenedFilesException ex)
        {
            MessageBoxResult result = MessageBox.Show(OpenedFilesException.WarningMessage, OpenedFilesException.WarningTitle,
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                await ex.ContinueAsync();
            }
        }

        private void Drive_UsernameWasNull()
        {
            MessageBox.Show("No ha indicado ningún nombre de usuario. Indique uno en los ajustes.", "Falta usuario",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            new Preferences(Service).ShowDialog();
        }

        private void EvirLinuxButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToLinuxDesktop();

        private void EvirWindowsButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToWindowsDesktop();

        private async Task ConnectPortalDSIC(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
                OpenPortalDSIC();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No ha indicado ningún nombre para la VPN del DSIC. Indique uno en los ajustes.",
                    "Falta nombre",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                new Preferences(Service).ShowDialog();
            }
            catch (OperationCanceledException)
            {
                // El usuario canceló algo, así que no importa
            }
        }

        private static void OpenPortalDSIC()
        {
            new Window()
            {
                Title = "Portal DSIC",
                Content = new WebBrowser
                {
                    Source = new Uri("http://" + VPNFactory.PORTAL_DSIC)
                }
            }.ShowDialog();
        }
    }
}
