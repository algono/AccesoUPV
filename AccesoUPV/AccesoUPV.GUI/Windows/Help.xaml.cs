using System;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Index_OnSelected(object sender, RoutedEventArgs e)
        {
            ListBoxItem entryItem = ((sender as ListBox).SelectedItem as ListBoxItem);

            object entry = entryItem.Tag;
            switch (entry)
            {
                case Type type:
                    // Crea una instancia de las paginas para la entrada seleccionada
                    entry = Activator.CreateInstance(type);
                    break;
                case Uri uri:
                    entry = new WebBrowser
                    {
                        Source = uri
                    };
                    break;
            }

            EntryFrame.Content = entry;
        }
    }
}
