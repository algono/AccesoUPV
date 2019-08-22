using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace AccesoUPV.GUI.Help
{
    /// <summary>
    /// Provider class for online help.  
    /// </summary>
    public static class HelpProvider
    {
        #region Fields

        public const string LocalHelp = "Help/AccesoUPV.chm",
            RemoteHelp = "Help/AccesoUPV-Remote.chm",
            RemoteHelpLink = "http://github.com/algono/AccesoUPV-help/releases/latest/download/AccesoUPV.chm";

        //private static bool _helpDownloaded;
        public static string Help
        {
            get
            {
                return LocalHelp;
                #region Download from remote
                //if (!_helpDownloaded) DownloadHelp();
                //return _helpDownloaded ? RemoteHelp : LocalHelp; 
                #endregion
            }
        }

        #region Remote Help (Git Repo)
        //private static void DownloadHelp()
        //{
        //    // Set cursor as hourglass
        //    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

        //    try
        //    {
        //        using (WebClient client = new WebClient())
        //        {
        //            client.DownloadFile(RemoteHelpLink, RemoteHelp);
        //        }

        //        _helpDownloaded = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // If the download fails, return the local help anyway
        //        Debug.WriteLine(ex.Message);
        //    }

        //    // Set cursor as default arrow
        //    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        //} 
        #endregion

        /// <summary>
        /// Help topic dependency property. 
        /// </summary>
        /// <remarks>This property can be attached to an object such as a form or a text box, and 
        /// can be retrieved when the user presses F1 and used to display context sensitive help.</remarks>
        public static readonly DependencyProperty HelpTopicProperty =
            DependencyProperty.RegisterAttached("HelpString", typeof(string), typeof(HelpProvider));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Static constructor that adds a command binding to Application.Help, binding it to 
        /// the CanExecute and Executed methods of this class. 
        /// </summary>
        /// <remarks>With this in place, when the user presses F1 our help will be invoked.</remarks>
        static HelpProvider()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(FrameworkElement),
                new CommandBinding(
                    ApplicationCommands.Help,
                    ShowHelpExecuted,
                    ShowHelpCanExecute));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Getter for <see cref="HelpTopicProperty"/>. Get a help topic that's attached to an object. 
        /// </summary>
        /// <param name="obj">The object that the help topic is attached to.</param>
        /// <returns>The help topic.</returns>
        public static string GetHelpTopic(DependencyObject obj)
        {
            return (string)obj.GetValue(HelpTopicProperty);
        }

        /// <summary>
        /// Setter for <see cref="HelpTopicProperty"/>. Attach a help topic value to an object. 
        /// </summary>
        /// <param name="obj">The object to which to attach the help topic.</param>
        /// <param name="value">The value of the help topic.</param>
        public static void SetHelpTopic(DependencyObject obj, string value)
        {
            obj.SetValue(HelpTopicProperty, value);
        }

        /// <summary>
        /// Show help table of contents. 
        /// </summary>
        public static void ShowHelpTableOfContents()
        {

            System.Windows.Forms.Help.ShowHelp(null, Help, HelpNavigator.TableOfContents);
        }

        /// <summary>
        /// Show a help topic in the online CHM style help. 
        /// </summary>
        /// <param name="helpTopic">The help topic to show. This must match exactly with the name 
        /// of one of the help topic's .htm files, without the .htm extension and with spaces instead of underscores
        /// in the name. For instance, to display the help topic "This_is_my_topic.htm", pass the string "This is my topic".</param>
        /// <remarks>You can also pass in the help topic with the underscore replacement already done. You can also 
        /// add the .htm extension. 
        /// Certain characters other than spaces are replaced by underscores in RoboHelp help topic names. 
        /// This method does not yet account for all those replacements, so if you really need to find a help topic
        /// with one or more of those characters, do the underscore replacement before passing the topic.</remarks>
        public static void ShowHelpTopic(string helpTopic)
        {
            // Strip off trailing period.
            if (helpTopic.IndexOf(".") == helpTopic.Length - 1)
                helpTopic = helpTopic.Substring(0, helpTopic.Length - 1);

            helpTopic = helpTopic.Replace(" ", "_").Replace("\\", "_").Replace("/", "_").Replace(":", "_").Replace("*", "_").Replace("?", "_").Replace("\"", "_").Replace(">", "_").Replace("<", "_").Replace("|", "_") + (helpTopic.IndexOf(".htm") == -1 ? ".htm" : "");
            System.Windows.Forms.Help.ShowHelp(null, Help, HelpNavigator.Topic, helpTopic);
        }

        /// <summary>
        /// Whether the F1 help command can execute. 
        /// </summary>
        private static void ShowHelpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            if (HelpProvider.GetHelpTopic(senderElement) != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Execute the F1 help command. 
        /// </summary>
        /// <remarks>Calls ShowHelpTopic to show the help topic attached to the framework element that's the 
        /// source of the call.</remarks>
        private static void ShowHelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ShowHelpTopic(HelpProvider.GetHelpTopic(sender as FrameworkElement));
        }

        #endregion Methods
    }
}
