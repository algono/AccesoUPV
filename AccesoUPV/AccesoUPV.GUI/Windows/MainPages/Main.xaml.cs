using AccesoUPV.GUI.UserControls;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.Disco_W.Username)))
            {
                Drive_UsernameWasNull();
            }
        }

        private async Task ConnectDSICDrive(object sender, ConnectionEventArgs e)
        {
            try
            {
                await e.ConnectionFunc();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.Disco_DSIC.Username)))
            {
                Drive_UsernameWasNull();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(Service.Disco_DSIC.Password)))
            {
                MessageBox.Show("No ha indicado ninguna contraseña. Indíquela en los ajustes.", "Falta contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
                new Preferences(Service).ShowDialog();
            }
        }

        private async Task DisconnectDrive(object sender, ConnectionEventArgs e)
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
            MessageBoxResult result = MessageBox.Show(ex.Message, OpenedFilesException.WarningTitle,
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

        private async void ConnectPortalDSIC(object sender, RoutedEventArgs e)
        {
            try
            {
                await Service.VPN_DSIC.ConnectAsync();

                #region Portal DSIC Dialog
                Window portalWindow = CreatePortalDSICDialog();
                portalWindow.Closed += async (s, ce) => await Service.VPN_DSIC.DisconnectAsync();
                portalWindow.Show();
                #endregion
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

        private static Window CreatePortalDSICDialog() => new Window()
        {
            Title = "Portal DSIC",
            Content = new WebBrowser
            {
                Source = new Uri("http://" + VPNFactory.PORTAL_DSIC)
            }
        };
    }
}
