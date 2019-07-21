using AccesoUPV.Library.Connectors.VPN;
using System.Windows;

namespace AccesoUPV.GUI
{
    /// <summary>
    /// Lógica de interacción para SelectVPN.xaml
    /// </summary>
    public partial class SelectVPN : Window
    {
        public IVPN VPN { get; private set; }

        public string SelectedName { get; private set; }

        public SelectVPN()
        {
            InitializeComponent();
        }

        public SelectVPN(IVPN vpn) : this()
        {
            VPN = vpn;
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
