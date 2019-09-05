using AccesoUPV.GUI.Help;
using AccesoUPV.GUI.UserControls;
using AccesoUPV.Library.Connectors;
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
                await ConnectDrive(sender, e);
            }
            catch (CredentialsBugException ex)
            {
                MessageBox.Show(ex.Message, "Error de credenciales", MessageBoxButton.OK, MessageBoxImage.Error);

                #region Reconexión UPV
                VPN VPN_UPV = Service.VPN_UPV;
                if (VPN_UPV.IsConnected)
                {
                    await VPN_UPV.DisconnectAsync();
                    await VPN_UPV.ConnectAsync();
                } 
                #endregion
            }
        }

        private async Task ConnectDrive(object sender, ConnectionEventArgs e)
        {
            NetworkDrive networkDrive = e.Connectable as NetworkDrive;
            try
            {
                await e.ConnectionFunc();
            }
            catch (NotAvailableDriveException ex) when (ex.Letter == default)
            {
                MessageBox.Show(ex.Message, "Ninguna unidad disponible", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotAvailableDriveException ex)
            {
                string WARNING_W =
                    ex.Message + "\n" +
                    "(Continúe si prefiere que se elija la primera unidad disponible solo durante esta conexión).\n ";

                MessageBoxResult result = MessageBox.Show(WARNING_W, $"Unidad {ex.Letter} contiene disco", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    networkDrive.Letter = default;
                    await e.ConnectionFunc().ContinueWith((t) => networkDrive.Letter = ex.Letter);
                }
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(networkDrive.Username)))
            {
                MessageBox.Show("No ha indicado ningún nombre de usuario. Indique uno en los ajustes.", "Falta usuario",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                new Preferences(Service).ShowDialog();
            }
            catch (ArgumentNullException ex) when (ex.ParamName.Equals(nameof(networkDrive.Password)))
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

        private void EvirLinuxButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToLinuxDesktop();

        private void EvirWindowsButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToWindowsDesktop();

        private async Task ConnectPortalDSIC(object sender, ConnectionEventArgs e)
        {
            try
            {
                #region Conflicto con Disco W
                if (Service.Disco_W.IsConnected)
                {
                    MessageBoxResult result = MessageBox.Show("No se puede acceder a la VPN del DSIC teniendo el Disco W conectado.\n"
                                    + "Si continúa, este será desconectado automáticamente.\n\n"
                                    + "¿Desea continuar?", "Conflicto entre conexiones", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        await Service.Disco_W.DisconnectAsync();
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }
                #endregion

                await e.ConnectionFunc();
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

        #region Website Window (Obsolete)
        [Obsolete]
        public static Window CreatePortalDSICDialog() => new Window()
        {
            Title = "Portal DSIC",
            Content = new WebBrowser
            {
                Source = new Uri("http://" + VPNFactory.PORTAL_DSIC),
                
            }
        };
        [Obsolete]
        public static void OpenConnectableWindow(IConnectable connectable, Window window)
        {
            window.Closed += async (s, ce) => await connectable.DisconnectAsync();
            window.ShowDialog();
        } 
        #endregion

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            new Preferences(Service).ShowDialog();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelpTableOfContents();
        }
    }
}
