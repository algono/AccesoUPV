using AccesoUPV.GUI.Windows.MainPages;
using AccesoUPV.Library.Services;
using System;
using System.ComponentModel;
using System.Windows;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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
            this.Closing += Shutdown;
        }

        private void Shutdown(object sender, CancelEventArgs e)
        {
            Shutdown shutdownWindow = new Shutdown(_service);
            shutdownWindow.Canceled += (s, ev) => e.Cancel = true;
            shutdownWindow.ShowDialog();

            // close all active threads (this prevents a weird TaskCanceledException from happening)
            if (!e.Cancel) Environment.Exit(0);
        }

    }
}
