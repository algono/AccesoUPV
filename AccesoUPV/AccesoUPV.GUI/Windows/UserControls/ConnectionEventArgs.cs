﻿using AccesoUPV.Library.Connectors;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace AccesoUPV.GUI.Windows.UserControls
{
    public class ConnectionEventArgs : EventArgs
    {
        public RoutedEventArgs RoutedEventArgs { get; set; }
        public Connectable Connectable { get; set; }
        public Func<Task> ConnectionFunc { get; set; }
    }
}
