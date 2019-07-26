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
            Application.Current.SessionEnding += Current_SessionEnding;
        }

        private async void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            await _service.Shutdown();
        }

        public Main(IAccesoUPVService service) : this()
        {
            _service = service;
        }
    }
}
