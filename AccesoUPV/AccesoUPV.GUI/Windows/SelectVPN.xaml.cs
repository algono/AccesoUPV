using AccesoUPV.Library.Connectors.VPN;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para SelectVPN.xaml
    /// </summary>
    public partial class SelectVPN
    {
        public VPN SelectedVPN { get; }
        public string SelectedName => SelectedVPN.Name;

        public string Server => SelectedVPN.Config.Server;

        public bool Canceled { get; private set; }

        private bool _allowCreation;
        public bool AllowCreation
        {
            get => _allowCreation;
            set
            {
                if (value)
                {
                    VPNList.Items.Insert(0, CreateNewVPNItemText);
                }
                else
                {
                    VPNList.Items.Remove(CreateNewVPNItemText);
                }

                _allowCreation = value;
            }
        }

        private const string CreateNewVPNItemText = @"<Crear nueva>";

        public SelectVPN(VPNConfig config, bool allowCreation = true) : this(new VPN(config), allowCreation)
        {

        }
        
        public SelectVPN(string server, bool allowCreation = false) : this(new VPN(server), allowCreation)
        {

        }

        public SelectVPN(VPN vpn, bool allowCreation = true)
        {
            InitializeComponent();
            SelectedVPN = vpn;
            _allowCreation = allowCreation;

            this.Loaded += async (sender, e) =>
            {
                await LoadNameList();
                ShowList();
            };
        }

        private async Task LoadNameList()
        {
            IList<string> list = Server == null
                ? await VPN.GetNameListAsync()
                : await VPNConfig.FindNamesAsync(Server);

            if (AllowCreation) list.Insert(0, CreateNewVPNItemText);
            VPNList.ItemsSource = list;
        }

        private void ShowList()
        {
            VPNProgressBar.Visibility = Visibility.Collapsed;
            VPNList.Visibility = Visibility.Visible;
        }

        private void HideList()
        {
            VPNProgressBar.Visibility = Visibility.Visible;
            VPNList.Visibility = Visibility.Collapsed;
        }

        private async void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AllowCreation && VPNList.SelectedIndex == 0)
                {
                    await CreateNewVPN();
                }
                else
                {
                    SelectedVPN.Name = VPNList.SelectedItem.ToString();
                }

                this.Close();
            }
            catch (OperationCanceledException)
            {
                // The user canceled the operation
            }
        }

        private async Task CreateNewVPN()
        {
            string newName = Interaction.InputBox("Introduzca el nombre de la nueva conexión VPN:", "Nueva conexión VPN");
            if (string.IsNullOrEmpty(newName)) throw new OperationCanceledException();
            else
            {
                SelectedVPN.Name = newName;

                // Set cursor as hourglass
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                await SelectedVPN.CreateAsync();

                // Set cursor as default arrow
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

                MessageBox.Show("La conexión VPN ha sido creada con éxito");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled = true;
            this.Close();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            HideList();
            await LoadNameList();
            ShowList();
        }
    }
}
