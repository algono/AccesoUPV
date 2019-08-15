using AccesoUPV.Library.Connectors.VPN;
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
        public string SelectedName { get; private set; }

        public bool DetectList { get; }

        private readonly string _server;
        public SelectVPN(string server = null)
        {
            InitializeComponent();
            _server = server;
            DetectList = true;
            this.Loaded += async (sender, e) =>
            {
                await LoadNameList();
                ShowList();
            };
        }
        public SelectVPN(IEnumerable<string> vpnList)
        {
            InitializeComponent();
            VPNList.ItemsSource = vpnList;
            DetectList = false;
            ShowList();
        }

        private async Task LoadNameList()
        {
            VPNList.ItemsSource = _server == null
                ? await VPN.GetNameListAsync()
                : await VPNConfig.FindNamesAsync(_server);
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

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedName = VPNList.SelectedItem.ToString();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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
