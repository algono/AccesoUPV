using AccesoUPV.GUI.Windows.MainPages;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const string ConnectionErrorMessage
            = "Ha habido un error al conectarse a la VPN. Inténtelo de nuevo.\n\n"
            + "Si el problema persiste, trate de conectarse de forma manual.";

        private bool started = false;
        private readonly IAccesoUPVService _service;
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IAccesoUPVService service) : this()
        {
            _service = service;

            Start startPage = new Start(service);
            ContentFrame.Navigate(startPage);
            this.Closing += Shutdown;
        }

        private async void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                object tag = (e.ClickedItem as HamburgerMenuItem).Tag;
                if (!started && tag is Type type && !typeof(Start).IsAssignableFrom(type))
                {
                    await Start();
                    started = true;
                }

                HamburgerMenu_ItemHandler(tag);
            }
            catch (OperationCanceledException)
            {
                (sender as HamburgerMenu).SelectedIndex = 0; // Reset selection to Home
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

        private void HamburgerMenu_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            object tag = (e.ClickedItem as HamburgerMenuItem).Tag;
            HamburgerMenu_ItemHandler(tag);
        }

        private void HamburgerMenu_ItemHandler(object tag)
        {
            if (tag is Type type)
            {
                if (typeof(Page).IsAssignableFrom(type))
                {
                    Page page = (Page)Activator.CreateInstance(type, _service);
                    ContentFrame.Navigate(page);
                }
                else if (typeof(Window).IsAssignableFrom(type))
                {
                    Window window = (Window)Activator.CreateInstance(type, _service);
                    window.ShowDialog();
                }
            }
            else if (tag is Action action)
            {
                action.Invoke();
            }
        }

        public async Task Start()
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
        }

        private void Shutdown(object sender, CancelEventArgs e)
        {
            Shutdown shutdownWindow = new Shutdown(_service);
            shutdownWindow.Canceled += (s, ev) => e.Cancel = true;
            shutdownWindow.ShowDialog();
        }

    }
}
