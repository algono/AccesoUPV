using AccesoUPV.GUI.Help;
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
    /// Lógica de interacción para UPVPage.xaml
    /// </summary>
    public partial class UPVPage : Page
    {
        public IAccesoUPVService Service { get; }

        public UPVPage()
        {
            InitializeComponent();
        }

        public UPVPage(IAccesoUPVService service)
        {
            Service = service;
            InitializeComponent();
        }

        private async Task ConnectWDrive(object sender, ConnectionEventArgs e)
            => await ConnectableHandlers.ConnectWDrive(Service, e);

        private async Task DisconnectDrive(object sender, ConnectionEventArgs e)
            => await ConnectableHandlers.DisconnectDrive(e);


        private void DiscaButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectableHandlers.ConnectToSSH(Service, SSHConnection.DISCA_SSH);
        }

    }
}
