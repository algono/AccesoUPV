using AccesoUPV.GUI.Help;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using System;
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
                    bool exists = !string.IsNullOrEmpty(vpn.Name) || await CreateVPN(vpn, "la UPV desde fuera del campus");
                    if (exists)
                    {
                        _service.SaveChanges();
                        await vpn.ConnectAsync();
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
                }

                OnStarted(EventArgs.Empty);
            }
            catch (OperationCanceledException)
            {
                // The user canceled the connection
            }
        }

        private static async Task<bool> CreateVPN(VPN vpn, string vpnMsgText = "la red")
        {
            MessageBoxResult result = MessageBox.Show(
                                $"Debe establecer una red VPN para poder acceder a {vpnMsgText}.\n\n"
                                + "¿Desea que se cree la VPN automáticamente?\n\n"
                                + "Si ya tiene una creada o prefiere crearla manualmente, elija 'No'.", "Establecer VPN", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning
                                );
            if (result == MessageBoxResult.Yes)
            {
                vpn.Name = Interaction.InputBox("Introduzca el nombre de la nueva conexión VPN:");
                await vpn.CreateAsync();
            }
            else if (result == MessageBoxResult.No)
            {
                SelectVPN window = new SelectVPN(vpn);
                window.ShowDialog();
                if (window.Canceled) return false;
                vpn.Name = window.SelectedName;
            }
            else
            {
                return false;
            }

            return true;
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
            // new Help().ShowDialog();
            HelpProvider.ShowHelpTableOfContents();
        }
    }
}
