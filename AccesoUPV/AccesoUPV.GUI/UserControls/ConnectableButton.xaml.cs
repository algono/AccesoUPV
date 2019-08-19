using AccesoUPV.Library.Connectors;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.UserControls
{
    /// <summary>
    /// Lógica de interacción para ConnectableButton.xaml
    /// </summary>
    public partial class ConnectableButton
    {
        #region Dependency Properties
        public Connectable Connectable
        {
            get => (Connectable)GetValue(ConnectableProperty);
            set => SetValue(ConnectableProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string IconKind
        {
            get => (string)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconKind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Connect"));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Conectar"));

        // Using a DependencyProperty as the backing store for Connectable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectableProperty =
            DependencyProperty.Register("Connectable", typeof(Connectable), typeof(ConnectableButton), new PropertyMetadata());

        #endregion

        #region Event Handlers
        public Func<object, ConnectionEventArgs, Task> ConnectHandler { get; set; }
        public Func<object, ConnectionEventArgs, Task> DisconnectHandler { get; set; }

        #endregion

        public ConnectableButton()
        {
            InitializeComponent();
        }

        private async Task Connect(object sender, RoutedEventArgs e)
            => await Handle(sender, e, StartConnecting, ConnectHandler);

        private async Task StartConnecting()
        {
            if (!Connectable.IsConnected) await Connectable.ConnectAsync();
        }

        private async Task Disconnect(object sender, RoutedEventArgs e)
            => await Handle(sender, e, Connectable.DisconnectAsync, DisconnectHandler);

        private async Task Handle(object sender, RoutedEventArgs e, Func<Task> function, Func<object, ConnectionEventArgs, Task> handler)
        {
            if (handler == null)
            {
                await function();
            }
            else
            {
                ConnectionEventArgs ce = new ConnectionEventArgs
                {
                    Connectable = Connectable,
                    ConnectionFunc = function,
                    RoutedEventArgs = e
                };
                await handler(sender, ce);
            }
        }

        private async void ConnectionSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (Connectable.IsConnected) await Disconnect(sender, e);
            else await Connect(sender, e);

            ConnectionSwitch.IsChecked = Connectable.IsConnected;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (Connectable.IsConnected && Connectable is Openable openable) openable.Open();
        }
    }
}
