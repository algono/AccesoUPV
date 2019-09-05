using AccesoUPV.Library.Connectors.Drive;
using System;
using System.Globalization;
using System.Windows.Data;

namespace AccesoUPV.GUI.UserControls.Preferences
{
    public class DriveLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DriveLetterTools.ToDriveLetter((char)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DriveLetterTools.FromDriveLetter(value.ToString());
        }
    }
}
