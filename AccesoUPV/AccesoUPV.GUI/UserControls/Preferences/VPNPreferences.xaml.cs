using AccesoUPV.Library.Connectors.VPN;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        #endregion
    }
}
