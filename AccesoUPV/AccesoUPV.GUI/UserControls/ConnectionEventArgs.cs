using AccesoUPV.Library.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace AccesoUPV.GUI.UserControls
{
    public class ConnectionEventArgs : EventArgs
    {
        public RoutedEventArgs RoutedEventArgs { get; set; }
        public IConnectable Connectable { get; set; }
        public Func<Task> ConnectionFunc { get; set; }

        public static ConnectionEventArgs CreateFrom(IConnectable connectable, bool connect)
        {
            Func<Task> connectionFunc;
            if (connect) connectionFunc = connectable.ConnectAsync;
            else connectionFunc = connectable.DisconnectAsync;

            return new ConnectionEventArgs
            {
                Connectable = connectable,
                ConnectionFunc = connectionFunc
            };
        }
    }
}
