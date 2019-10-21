using AccesoUPV.GUI.Static;
using AccesoUPV.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AccesoUPV.GUI.UserControls
{
    /// <summary>
    /// Lógica de interacción para ConnectableButton.xaml
    /// </summary>
    public partial class ConnectableButton
    {
        #region Dependency Properties
        public IConnectable Connectable
        {
            get => (IConnectable)GetValue(ConnectableProperty);
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

        public bool IsConnected
        {
            get => (bool)GetValue(IsConnectedProperty);
            set => SetValue(IsConnectedProperty, value);
        }


        #region Progress
        public bool IsProgressIndeterminate
        {
            get => (bool)GetValue(IsProgressIndeterminateProperty);
            set => SetValue(IsProgressIndeterminateProperty, value);
        }

        // Range: 0.0 to 1.0
        public double ProgressBarOpacity
        {
            get => (double)GetValue(ProgressBarOpacityProperty);
            set => SetValue(ProgressBarOpacityProperty, value);
        }

        public double ProgressMinimum
        {
            get => (double)GetValue(ProgressMinimumProperty);
            set => SetValue(ProgressMinimumProperty, value);
        }

        public double ProgressMaximum
        {
            get => (double)GetValue(ProgressMaximumProperty);
            set => SetValue(ProgressMaximumProperty, value);
        }

        // Using a DependencyProperty as the backing store for ProgressMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressMaximumProperty =
            DependencyProperty.Register("ProgressMaximum", typeof(double), typeof(ConnectableButton), new PropertyMetadata(1.0));

        // Using a DependencyProperty as the backing store for ProgressMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressMinimumProperty =
            DependencyProperty.Register("ProgressMinimum", typeof(double), typeof(ConnectableButton), new PropertyMetadata(0.0));

        // Using a DependencyProperty as the backing store for ProgressBarOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressBarOpacityProperty =
            DependencyProperty.Register("ProgressBarOpacity", typeof(double), typeof(ConnectableButton), new PropertyMetadata(0.5));

        // Using a DependencyProperty as the backing store for IsProgressIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsProgressIndeterminateProperty =
            DependencyProperty.Register("IsProgressIndeterminate", typeof(bool), typeof(ConnectableButton), new PropertyMetadata(true)); 
        #endregion


        // Using a DependencyProperty as the backing store for IsConnected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(ConnectableButton), new UIPropertyMetadata());

        // Using a DependencyProperty as the backing store for IconKind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Connect"));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ConnectableButton), new PropertyMetadata("Conectar"));

        // Using a DependencyProperty as the backing store for Connectable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectableProperty =
            DependencyProperty.Register("Connectable", typeof(IConnectable), typeof(ConnectableButton), new PropertyMetadata(OnConnectableChanged));


        private static void OnConnectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ConnectableHandlers.Unbind(d, (IConnectable)e.OldValue);
            }

            if (e.NewValue != null)
            {
                ConnectableHandlers.Bind(d, (IConnectable)e.NewValue,
                    ConnectableHandlers.CreateOnConnectionStatusChanged(d, IsConnectedProperty));
            }
        }

        #endregion

        #region Event Handlers
        public Func<object, ConnectionEventArgs, Task> ConnectHandler { get; set; }
        public Func<object, ConnectionEventArgs, Task> DisconnectHandler { get; set; }
        public Action<IConnectable> OpenHandler { get; set; } = TryToOpen;

        #endregion

        public ConnectableButton()
        {
            InitializeComponent();
            this.Loaded += (s, e) => UpdateIsConnectedPropertyState();
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
            try
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
            catch (OperationCanceledException)
            {
                // El usuario canceló algo, así que no importa
            }
        }

        private async void ConnectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ConnectionCheckBox.IsEnabled = false;
            StatusProgressBar.Visibility = Visibility.Visible;

            UpdateCheckBoxState(); 

            try
            {
                if (Connectable.IsConnected)
                {
                    await Disconnect(sender, e);
                }
                else
                {
                    await Connect(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            StatusProgressBar.Visibility = Visibility.Collapsed;
            ConnectionCheckBox.IsEnabled = true;
        }

        /**
        * The CheckBox changes the checked state the moment you click it, so it needs to be updated to
        * match the actual value of IsConnected
        */
        private void UpdateCheckBoxState()
        {
            ConnectionCheckBox.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateTarget();
        }

        private void UpdateIsConnectedPropertyState()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetValue(IsConnectedProperty, Connectable?.IsConnected ?? false);
            });
        }

        private void TryToOpen_Click(object sender, RoutedEventArgs e) => OpenHandler(Connectable);

        private static void TryToOpen(IConnectable connectable)
        {
            if (connectable.IsConnected && connectable is IOpenable openable) openable.Open();
        }
    }
}
