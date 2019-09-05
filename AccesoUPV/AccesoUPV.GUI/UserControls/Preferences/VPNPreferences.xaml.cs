using AccesoUPV.GUI.Windows;
using AccesoUPV.Library.Connectors.VPN;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AccesoUPV.GUI.UserControls.Preferences
{
    /// <summary>
    /// Lógica de interacción para VPNPreferences.xaml
    /// </summary>
    [ContentProperty("Title")]
    public partial class VPNPreferences : UserControl
    {
        #region Dependency Properties
        public VPN VPN
        {
            get => (VPN)GetValue(VPNProperty);
            set => SetValue(VPNProperty, value);
        }

        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(VPNPreferences), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for VPN.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VPNProperty =
            DependencyProperty.Register("VPN", typeof(VPN), typeof(VPNPreferences), new PropertyMetadata());
        #endregion

        public VPNPreferences()
        {
            InitializeComponent();
        }

        #region Public Methods
        public void Load()
        {
            NameBox.Text = VPN.Name;
        }
        public void Save()
        {
            VPN.Name = NameBox.Text;
        }

        public void Clear()
        {
            NameBox.Clear();
        }

        public void Select()
        {
            SelectVPN window = new SelectVPN(VPN);
            window.ShowDialog();

            if (!window.Canceled) NameBox.Text = window.SelectedName;
        }
        #endregion

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }
    }
}
