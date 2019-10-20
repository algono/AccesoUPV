using AccesoUPV.GUI.Static;
using AccesoUPV.GUI.Windows.MainPages;
using AccesoUPV.Library.Connectors.VPN;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AccesoUPV.GUI.Windows
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const string ConnectionErrorMessage
            = "Ha habido un error al conectarse a la VPN. Inténtelo de nuevo.\n\n"
            + "Si el problema persiste, trate de conectarse de forma manual.";

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private bool started = false;
        private readonly IAccesoUPVService _service;
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IAccesoUPVService service) : this()
        {
            _service = service;

            notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                BalloonTipText = "La aplicación ha sido minimizada. Use el icono de la barra de tareas para volver a mostrarla.",
                BalloonTipTitle = Title,
                Text = Title,
                Icon = new System.Drawing.Icon("app-icon.ico")
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            BuildNotifyIconContextMenu();

            Start startPage = new Start(service);
            ContentFrame.Navigate(startPage);
        }

        private void BuildNotifyIconContextMenu()
        {
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            notifyIcon.ContextMenuStrip.Items.Add("Abrir", null, (s, e) => ShowWindowFromNotifyIcon());
            notifyIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            var connectToolStrip = new System.Windows.Forms.ToolStripMenuItem("Conectar");

            connectToolStrip.DropDownItems.Add("Disco W", null, async (s, e) => await ConnectableHandlers.ConnectWDrive(_service, UserControls.ConnectionEventArgs.CreateFrom(_service.Disco_W, true)));
            connectToolStrip.DropDownItems.Add("DISCA", null, (s, e) => ConnectableHandlers.ConnectToSSH(_service, SSHConnection.DISCA_SSH));

            var dsicToolStrip = new System.Windows.Forms.ToolStripMenuItem("DSIC");

            dsicToolStrip.DropDownItems.Add("Asig (M:)", null, async (s, e) => await ConnectableHandlers.ConnectDrive(_service, UserControls.ConnectionEventArgs.CreateFrom(_service.Asig_DSIC, true)));
            dsicToolStrip.DropDownItems.Add("Homes (W:)", null, async (s, e) => await ConnectableHandlers.ConnectDrive(_service, UserControls.ConnectionEventArgs.CreateFrom(_service.Disco_DSIC, true)));
            dsicToolStrip.DropDownItems.Add("Portal", null, async (s, e) => await ConnectableHandlers.ConnectPortalDSIC(_service, UserControls.ConnectionEventArgs.CreateFrom(_service.VPN_DSIC, true)));

            var evirToolStrip = new System.Windows.Forms.ToolStripMenuItem("Escritorios Remotos");
            evirToolStrip.DropDownItems.Add("Linux", null, (s, e) => RemoteDesktop.ConnectToLinuxDesktop());
            evirToolStrip.DropDownItems.Add("Windows", null, (s, e) => RemoteDesktop.ConnectToWindowsDesktop());

            dsicToolStrip.DropDownItems.Add(evirToolStrip);

            dsicToolStrip.DropDownItems.Add("Kahan", null, (s, e) => ConnectableHandlers.ConnectToSSH(_service, SSHConnection.KAHAN_SSH));

            connectToolStrip.DropDownItems.Add(dsicToolStrip);

            notifyIcon.ContextMenuStrip.Items.Add(connectToolStrip);
        }

        private async void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                object tag = (e.ClickedItem as HamburgerMenuItem).Tag;
                if (!started && tag is Type type && !typeof(Start).IsAssignableFrom(type))
                {
                    await Start();
                    started = true;
                }

                HamburgerMenu_ItemHandler(tag);
            }
            catch (OperationCanceledException)
            {
                (sender as HamburgerMenu).SelectedIndex = 0; // Reset selection to Home
            }
            catch (IOException)
            {
                MessageBox.Show(
                    ConnectionErrorMessage,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void HamburgerMenu_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            object tag = (e.ClickedItem as HamburgerMenuItem).Tag;
            HamburgerMenu_ItemHandler(tag);
        }

        private void HamburgerMenu_ItemHandler(object tag)
        {
            if (tag is Type type)
            {
                if (typeof(Page).IsAssignableFrom(type))
                {
                    Page page = (Page)Activator.CreateInstance(type, _service);
                    ContentFrame.Navigate(page);
                }
                else if (typeof(Window).IsAssignableFrom(type))
                {
                    Window window = (Window)Activator.CreateInstance(type, _service);
                    window.ShowDialog();
                }
            }
            else if (tag is Action action)
            {
                action.Invoke();
            }
        }

        public async Task Start()
        {
            VPN vpn = _service.VPN_UPV;
            if (!vpn.IsReachable())
            {
                if (string.IsNullOrEmpty(vpn.Name))
                {
                    SelectVPN window = new SelectVPN(vpn);
                    window.ShowDialog();
                    if (window.Canceled) throw new OperationCanceledException();
                }

                _service.SaveChanges();
                await vpn.ConnectAsync();
            }
        }

        private void Shutdown(object sender, CancelEventArgs e)
        {
            Shutdown shutdownWindow = new Shutdown(_service);
            shutdownWindow.Canceled += (s, ev) => e.Cancel = true;
            shutdownWindow.ShowDialog();

            if (!e.Cancel)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }

        private WindowState storedWindowState = WindowState.Normal;
        private bool showedBalloonTip = false;
        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (notifyIcon != null)
                {
                    if (!showedBalloonTip)
                    {
                        notifyIcon.ShowBalloonTip(2000);
                        showedBalloonTip = true;
                    }
                }
                else
                {
                    storedWindowState = WindowState;
                }
            }
        }

        private void MetroWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = !IsVisible;
            }
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ShowWindowFromNotifyIcon();
            }
        }

        private void ShowWindowFromNotifyIcon()
        {
            Show();
            WindowState = storedWindowState;
        }
    }
}
