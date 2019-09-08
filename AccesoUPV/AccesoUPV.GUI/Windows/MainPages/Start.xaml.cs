using AccesoUPV.GUI.Help;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows.MainPages
{
    /// <summary>
    /// Lógica de interacción para Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public event EventHandler Started;
        public const string ConnectionErrorMessage = "Ha habido un error al conectarse a la VPN. Inténtelo de nuevo.\n\n Si el problema persiste, trate de conectarse de forma manual.";

        private readonly IAccesoUPVService _service;

        public Start()
        {
            InitializeComponent();
        }

        public Start(IAccesoUPVService service) : this()
        {
            _service = service;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VPN vpn = _service.VPN_UPV;
                if (!vpn.IsReachable())
                {
                    if (string.IsNullOrEmpty(vpn.Name))
                    {
                        SelectVPN window = new SelectVPN(vpn);
                        window.ShowDialog();
                        if (window.Canceled) throw new OperationCanceledException();
                    }

                    _service.SaveChanges();
                    await vpn.ConnectAsync();
                }

                OnStarted(EventArgs.Empty);
            }
            catch (OperationCanceledException)
            {
                // The user canceled the connection
            }
            catch (IOException)
            {
                MessageBox.Show(
                    ConnectionErrorMessage,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            Window preferencesWindow = new Preferences(_service);
            preferencesWindow.ShowDialog();
        }

        protected virtual void OnStarted(EventArgs e)
        {
            Started?.Invoke(this, e);
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelpTableOfContents();
        }
    }
}
