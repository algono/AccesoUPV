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

        public bool SavePasswords
        {
            get => (bool)GetValue(SavePasswordsProperty);
            set => SetValue(SavePasswordsProperty, value);
        }

        public Visibility PasswordOptionsVisibility
        {
            get => (Visibility)GetValue(PasswordOptionsVisibilityProperty);
            set => SetValue(PasswordOptionsVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for PasswordOptionsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordOptionsVisibilityProperty =
            DependencyProperty.Register("PasswordOptionsVisibility", typeof(Visibility), typeof(DrivePreferences), new PropertyMetadata(Visibility.Visible));



        // Using a DependencyProperty as the backing store for SavePasswords.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SavePasswordsProperty =
            DependencyProperty.Register("SavePasswords", typeof(bool), typeof(DrivePreferences), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(DrivePreferences), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for VPN.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DriveProperty =
            DependencyProperty.Register("Drive", typeof(NetworkDrive), typeof(DrivePreferences), new PropertyMetadata());
        #endregion

        public DrivePreferences()
        {
            InitializeComponent();
        }

        #region Public Methods
        public void Load()
        {
            LoadDriveBox();

            string driveLetter = Drive.Drive;
            if (!string.IsNullOrEmpty(driveLetter))
            {
                DriveCheckBox.IsChecked = true;
                DriveBox.SelectedItem = driveLetter;
            }

            ShowOnlyAvailableDrives.Click += (s, e) =>
            {
                if (ShowOnlyAvailableDrives.IsChecked ?? false)
                {
                    DriveBox.ItemsSource
                    = NetworkDrive.SelectAvailable(
                        DriveBox.ItemsSource
                        as IEnumerable<string>);
                }
                else
                {
                    LoadDriveBox();
                }
            };

            PassBox.Password = Drive.Password;
            SavePassCheckBox.IsChecked = SavePasswords;
        }

        private void LoadDriveBox()
        {
            bool onlyIfAvailable = ShowOnlyAvailableDrives.IsChecked ?? false;
            DriveBox.ItemsSource = NetworkDrive.GetDrives(onlyIfAvailable);
        }

        public void Save()
        {
            Drive.Drive =
                (DriveCheckBox.IsChecked ?? false)
                ? DriveBox.SelectedItem.ToString()
                : null;

            Drive.Password = PassBox.Password;
            SavePasswords = SavePassCheckBox.IsChecked ?? false;
        }

        public void Clear()
        {
            DriveBox.ItemsSource = null;
            DriveCheckBox.IsChecked = null;
            ShowOnlyAvailableDrives.IsChecked = null;
            SavePassCheckBox.IsChecked = null;
            PassBox.Clear();
        }
        #endregion

    }
}
