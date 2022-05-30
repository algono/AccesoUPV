using AccesoUPV.Library.Connectors.Drive;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AccesoUPV.GUI.UserControls.Preferences
{
    /// <summary>
    /// Lógica de interacción para DrivePreferences.xaml
    /// </summary>
    [ContentProperty("Title")]
    public partial class DrivePreferences : UserControl
    {
        #region Dependency Properties
        public NetworkDrive Drive
        {
            get => (NetworkDrive)GetValue(DriveProperty);
            set => SetValue(DriveProperty, value);
        }

        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Visibility PasswordOptionsVisibility
        {
            get => (Visibility)GetValue(PasswordOptionsVisibilityProperty);
            set => SetValue(PasswordOptionsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for PasswordOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordOptionsVisibilityProperty =
            DependencyProperty.Register("PasswordOptionsVisibility", typeof(Visibility), typeof(DrivePreferences), new PropertyMetadata(Visibility.Visible));


        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(DrivePreferences), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for VPN.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DriveProperty =
            DependencyProperty.Register("Drive", typeof(NetworkDrive), typeof(DrivePreferences), new PropertyMetadata());
        #endregion

        private const string DUMMY_PASSWORD = "--------";
        private bool _passwordChanged;

        public DrivePreferences()
        {
            InitializeComponent();
            LoadDriveBox();
        }

        #region Public Methods
        public void Load()
        {
            char driveLetter = Drive.Letter;
            if (DriveLetterTools.IsValid(driveLetter))
            {
                DriveCheckBox.IsChecked = true;
                DriveBox.SelectedItem = driveLetter;
            }

            if (Drive.AreCredentialsStored) PassBox.Password = DUMMY_PASSWORD;
            PassBox.PasswordChanged += (_, __) => _passwordChanged = true;
        }

        private void LoadDriveBox()
        {
            bool onlyIfAvailable = ShowOnlyAvailableDrives.IsChecked ?? false;
            DriveBox.ItemsSource = DriveLetterTools.GetDriveLetters(onlyIfAvailable);
        }

        public void Save()
        {
            Drive.Letter =
                (DriveCheckBox.IsChecked ?? false)
                ? (char)DriveBox.SelectedItem
                : default;

            if (_passwordChanged) Drive.SecurePassword = PassBox.SecurePassword;
        }

        public void Clear()
        {
            DriveBox.ItemsSource = null;
            DriveCheckBox.IsChecked = null;
            ShowOnlyAvailableDrives.IsChecked = null;
            PassBox.Clear();
        }
        #endregion

        private void ShowOnlyAvailableDrives_Click(object sender, RoutedEventArgs e)
        {
            if (ShowOnlyAvailableDrives.IsChecked ?? false)
            {
                DriveBox.ItemsSource
                = NetworkDrive.SelectAvailable(
                    DriveBox.ItemsSource
                    as IEnumerable<char>);
            }
            else
            {
                LoadDriveBox();
}
        }

    }
}
