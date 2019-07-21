using AccesoUPV.Library.Connectors.VPN;
using System.Windows;

namespace AccesoUPV.GUI
{
    /// <summary>
    /// Lógica de interacción para SelectVPN.xaml
    /// </summary>
    public partial class SelectVPN : Window
    {
        public string SelectedName { get; private set; }

        private string _server;
        public SelectVPN(string server = null)
        {
            InitializeComponent();
            _server = server;
            this.Loaded += LoadNameList;
        }

        private async void LoadNameList(object sender, RoutedEventArgs e)
        {
            VPNList.ItemsSource = _server == null 
                ? await VPN.GetNameListAsync()
                : await VPN.FindNamesAsync(_server);

            VPNProgressBar.Visibility = Visibility.Collapsed;
            VPNList.Visibility = Visibility.Visible;
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
    }
}
