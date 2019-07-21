using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows.Main.Pages
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
                IVPN vpn = _service.VPN_UPV;
                if (!vpn.IsReachable())
                {
                    bool exists = await vpn.SetNameAutoAsync()
                            || await CreateVPN(vpn, "la UPV desde fuera del campus");

                    if (exists)
                    {
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

        private async Task<bool> CreateVPN(IVPN vpn, string vpnMsgText = "la red")
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
                SelectVPN window = new SelectVPN();
                window.ShowDialog();
                if (window.SelectedName == null) return false;
                else vpn.Name = window.SelectedName;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void PrefsButton_Click(object sender, RoutedEventArgs e)
        {
            Window prefsWindow = new Preferences();
            prefsWindow.ShowDialog();
        }

        protected virtual void OnStarted(EventArgs e)
        {
            EventHandler handler = Started;
            handler?.Invoke(this, e);
        }
    }
}
