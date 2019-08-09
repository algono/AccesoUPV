using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AccesoUPV.Library.Connectors;

namespace AccesoUPV.GUI.Windows.UserControls
{
    public class ConnectionEventArgs : EventArgs
    {
        public RoutedEventArgs RoutedEventArgs { get; set; }
        public Connectable Connectable { get; set; }
        public Func<Task> ConnectionFunc { get; set; }
    }
}
