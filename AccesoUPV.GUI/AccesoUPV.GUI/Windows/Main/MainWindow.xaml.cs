using AccesoUPV.GUI.Windows.Main.Pages;
using AccesoUPV.Library.Services;
using System;
using System.Windows;

namespace AccesoUPV.GUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccesoUPVService _service;
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IAccesoUPVService service) : this()
        {
            _service = service;
            Start startPage = new Start(service);
            startPage.Started += StartPage_Started;
            Main.Content = startPage;
        }

        private void StartPage_Started(object sender, EventArgs e)
        {
            Main.Content = new Main(_service);
        }
        
    }
}
