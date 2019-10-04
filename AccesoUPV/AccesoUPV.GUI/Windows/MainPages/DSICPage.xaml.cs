using AccesoUPV.GUI.Static;
using AccesoUPV.GUI.UserControls;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows.MainPages
{
    /// <summary>
    /// Lógica de interacción para DSICPage.xaml
    /// </summary>
    public partial class DSICPage : Page
    {
        public bool IsPortalModeDisabled
        {
            get => (bool)GetValue(IsPortalModeDisabledProperty);
            private set => SetValue(IsPortalModeDisabledProperty, value);
        }
        // Using a DependencyProperty as the backing store for IsPortalModeDisabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPortalModeDisabledProperty =
            DependencyProperty.Register("IsPortalModeDisabled", typeof(bool), typeof(DSICPage), new PropertyMetadata(true));

        public IAccesoUPVService Service { get; }
        public DSICPage()
        {
            InitializeComponent();
        }

        public DSICPage(IAccesoUPVService service)
        {
            Service = service;
            InitializeComponent();

            ConnectableHandlers.Bind(this, Service.VPN_DSIC,
                ConnectableHandlers.CreateOnConnectionStatusChanged(this, IsPortalModeDisabledProperty, true));
        }

        public static bool IsPortalModeDisabledIn(IAccesoUPVService service)
            => !(service?.VPN_DSIC?.IsConnected ?? false);

        private async Task ConnectDrive(object sender, ConnectionEventArgs e)
            => await ConnectableHandlers.ConnectDrive(Service, e);

        private async Task DisconnectDrive(object sender, ConnectionEventArgs e)
            => await ConnectableHandlers.DisconnectDrive(e);

        private async Task ConnectPortalDSIC(object sender, ConnectionEventArgs e)
            => await ConnectableHandlers.ConnectPortalDSIC(Service, e);

        private void EvirLinuxButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToLinuxDesktop();

        private void EvirWindowsButton_Click(object sender, RoutedEventArgs e) => RemoteDesktop.ConnectToWindowsDesktop();

        private void KahanButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectableHandlers.ConnectToSSH(Service, SSHConnection.KAHAN_SSH);
        }
    }
}
