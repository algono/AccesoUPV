using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccesoUPV.GUI.Windows.Main.Pages
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        private readonly IAccesoUPVService _service;

        public Main()
        {
            InitializeComponent();
        }

        public Main(IAccesoUPVService service) : this()
        {
            _service = service;
        }

        private async void ConnectWButton_Click(object sender, RoutedEventArgs e)
        {
            await _service.WDrive.ConnectAsync();
        }

        private async void DisconnectWButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _service.WDrive.DisconnectAsync();
            }
            catch (OpenedFilesException ex)
            {
                MessageBoxResult result = MessageBox.Show(OpenedFilesException.WarningMessage, OpenedFilesException.WarningTitle, MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    await ex.ContinueAsync();
                }
            }
        }

        private void OpenWButton_Click(object sender, RoutedEventArgs e)
        {
            _service.WDrive.Open();
        }
    }
}
