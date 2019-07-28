using AccesoUPV.GUI.Windows;
using AccesoUPV.Library.Services;
using System.Windows;

namespace AccesoUPV.GUI
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AccesoUPVService service = new AccesoUPVService();
            MainWindow window = new MainWindow(service);
            window.Show();
        }
    }
}
