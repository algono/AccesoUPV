using AccesoUPV.Library.Connectors.Drive;
using AccesoUPV.Library.Services;
using System;
using System.Windows;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para Shutdown.xaml
    /// </summary>
    public partial class Shutdown : Window
    {
        private readonly IAccesoUPVService _service;

        public event EventHandler Finished, Canceled;

        public Shutdown()
        {
            InitializeComponent();
        }

        public Shutdown(IAccesoUPVService service) : this()
        {
            _service = service;
            this.Loaded += Shutdown_Loaded;
        }

        private async void Shutdown_Loaded(object sender, RoutedEventArgs e)
        {
            Progress<string> progress = new Progress<string>(Progress_ProgressChanged);
            // ShutdownProgressBar.Maximum = AccesoUPVService.NumberOfConnectables;

            bool done = false, canceled = false;
            while (!done)
            {
                try
                {
                    await _service.ShutdownAsync(progress);
                    done = true;
                }
                catch (OpenedFilesException ex)
                {
                    MessageBoxResult result = MessageBox.Show(OpenedFilesException.WarningMessage, OpenedFilesException.WarningTitle, MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        await ex.ContinueAsync();
                    }
                    else
                    {
                        done = true;
                        canceled = true;
                        OnCanceled(EventArgs.Empty);
                    }
                }
            }
            if (!canceled) OnFinished(EventArgs.Empty);
            this.Close();
        }

        private void Progress_ProgressChanged(string report)
        {
            ProgressTextBlock.Text = report + "...";
            ShutdownProgressBar.Value++;
        }

        protected virtual void OnFinished(EventArgs e)
        {
            Finished?.Invoke(this, e);
        }

        protected virtual void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }
    }
}
