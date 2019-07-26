using AccesoUPV.GUI.Windows.Main.Pages;
using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System;
using System.ComponentModel;
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
            this.Closing += Shutdown;
        }

        private void Shutdown(object sender, CancelEventArgs e)
        {
            bool done = false;
            while (!done)
            {
                try
                {
                    _service.Shutdown();
                    done = true;
                }
                catch (OpenedFilesException ex)
                {
                    MessageBoxResult result = MessageBox.Show(OpenedFilesException.WarningMessage, OpenedFilesException.WarningTitle, MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        ex.Continue();
                    }
                    else
                    {
                        e.Cancel = true;
                        done = true;
                    }
                }
            }
        }
    }
}
