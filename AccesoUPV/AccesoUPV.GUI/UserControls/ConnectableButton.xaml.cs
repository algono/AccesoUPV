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
    public partial class ConnectableButton : UserControl
    {
        #region Dependency Properties
        public Connectable Connectable
        {
            get => (Connectable)GetValue(ConnectableProperty);
            set => SetValue(ConnectableProperty, value);
        }

        public string ConnectText
        {
            get => (string)GetValue(ConnectTextProperty);
            set => SetValue(ConnectTextProperty, value);
        }

        public string DisconnectText
        {
            get => (string)GetValue(DisconnectTextProperty);
            set => SetValue(DisconnectTextProperty, value);
        }

        public int ConnectFontSize
        {
            get => (int)GetValue(ConnectFontSizeProperty);
            set => SetValue(ConnectFontSizeProperty, value);
        }

        public int DisconnectFontSize
        {
            get => (int)GetValue(DisconnectFontSizeProperty);
            set => SetValue(DisconnectFontSizeProperty, value);
        }

        public string ConnectIconKind
        {
            get => (string)GetValue(ConnectIconKindProperty);
            set => SetValue(ConnectIconKindProperty, value);
        }

        public string DisconnectIconKind
        {
            get => (string)GetValue(DisconnectIconKindProperty);
            set => SetValue(DisconnectIconKindProperty, value);
        }


        // Using a DependencyProperty as the backing store for Connectable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectableProperty =
            DependencyProperty.Register("Connectable", typeof(Connectable), typeof(ConnectableButton), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for ConnectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectTextProperty =
            DependencyProperty.Register("ConnectText", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Conectar"));

        // Using a DependencyProperty as the backing store for DisconnectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisconnectTextProperty =
            DependencyProperty.Register("DisconnectText", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Desconectar"));

        // Using a DependencyProperty as the backing store for ConnectFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectFontSizeProperty =
            DependencyProperty.Register("ConnectFontSize", typeof(int), typeof(ConnectableButton), new PropertyMetadata(14));

        // Using a DependencyProperty as the backing store for DisconnectFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisconnectFontSizeProperty =
            DependencyProperty.Register("DisconnectFontSize", typeof(int), typeof(ConnectableButton), new PropertyMetadata(11));

        // Using a DependencyProperty as the backing store for ConnectIconKind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectIconKindProperty =
            DependencyProperty.Register("ConnectIconKind", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Connect"));

        // Using a DependencyProperty as the backing store for DisconnectIconKind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisconnectIconKindProperty =
            DependencyProperty.Register("DisconnectIconKind", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Power"));

        #endregion

        #region Event Handlers
        public Func<object, ConnectionEventArgs, Task> ConnectHandler { get; set; }
        public Func<object, ConnectionEventArgs, Task> DisconnectHandler { get; set; }

        #endregion

        public ConnectableButton()
        {
            InitializeComponent();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectHandler == null)
            {
                await Connect();
            }
            else
            {
                ConnectionEventArgs ce = new ConnectionEventArgs
                {
                    Connectable = Connectable,
                    ConnectionFunc = Connect,
                    RoutedEventArgs = e
                };
                await ConnectHandler(sender, ce);
            }

            if (Connectable.IsConnected) DisconnectButton.Visibility = Visibility.Visible;
        }

        private async Task Connect()
        {
            if (!Connectable.IsConnected) await Connectable.ConnectAsync();
            if (Connectable is Openable openable) openable.Open();
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisconnectHandler == null)
            {
                await Disconnect();
            }
            else
            {
                ConnectionEventArgs ce = new ConnectionEventArgs
                {
                    Connectable = Connectable,
                    ConnectionFunc = Disconnect,
                    RoutedEventArgs = e
                };
                await DisconnectHandler(sender, ce);
            }

            if (!Connectable.IsConnected) DisconnectButton.Visibility = Visibility.Hidden;
        }

        private async Task Disconnect()
        {
            await Connectable.DisconnectAsync();
        }
    }
}
