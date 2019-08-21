using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccesoUPV.GUI.UserControls.Preferences
{
    [ValueConversion(typeof(Visibility), typeof(GridLength))]
    public class VisibilityToGridRowHeightConverter : DependencyObject, IValueConverter
    {
        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(VisibilityToGridRowHeightConverter), new PropertyMetadata(0.0));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Collapsed ? new GridLength(0) : new GridLength(Height, GridUnitType.Pixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need any convert back
            return null;
        }
    }
}
