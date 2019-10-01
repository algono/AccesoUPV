using AccesoUPV.GUI.Windows.MainPages;
using AccesoUPV.Library.Services;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
            ContentFrame.Navigate(startPage);
        }

        private void StartPage_Started(object sender, EventArgs e)
        {
            ContentFrame.Navigate(new Main(_service));
            this.Closing += Shutdown;
        }

        private void Shutdown(object sender, CancelEventArgs e)
        {
            Shutdown shutdownWindow = new Shutdown(_service);
            shutdownWindow.Canceled += (s, ev) => e.Cancel = true;
            shutdownWindow.ShowDialog();
        }

        private void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            object tag = (e.ClickedItem as HamburgerMenuItem).Tag;
            if (tag is Type type)
            {
                if (typeof(Page).IsAssignableFrom(type))
                {
                    Page page = (Page)Activator.CreateInstance(type, _service);
                    ContentFrame.Navigate(page);
                }
                else if (typeof(Window).IsAssignableFrom(type))
                {
                    Window window = (Window)Activator.CreateInstance(type, _service);
                    window.ShowDialog();
                }
            }
            else if (tag is Action action)
            {
                action.Invoke();
            }
        }

    }
}
