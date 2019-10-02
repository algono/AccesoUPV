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
        private readonly IAccesoUPVService _service;

        public Start()
        {
            InitializeComponent();
        }

        public Start(IAccesoUPVService service)
        {
            _service = service;
            InitializeComponent();
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            Window preferencesWindow = new Preferences(_service);
            preferencesWindow.ShowDialog();
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            HelpProvider.ShowHelpTableOfContents();
        }
    }
}
