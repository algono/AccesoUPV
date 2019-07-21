using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;
using System;
using System.Windows.Forms;

namespace AccesoUPV.Debug.GUI
{
    public partial class Principal : Form
    {
        private readonly AccesoUPVService _service;

        public Principal()
        {
            InitializeComponent();

        }

        public Principal(AccesoUPVService service) : this()
        {
            _service = service;

            _service.VPN_UPV.SetNameAuto();
            _service.VPN_DSIC.SetNameAuto();

            this.Load += InitVPN;
            this.FormClosing += DisconnectVPN;

            InitializeTextBoxes();
            InitializeConnectList();
        }

        private async void DisconnectVPN(object sender, FormClosingEventArgs e)
        {
            await _service.VPN_UPV.DisconnectAsync();
        }

        private void InitVPN(object sender, EventArgs e)
        {
            if (!_service.VPN_UPV.IsReachable())
            {
                try
                {
                    _service.VPN_UPV.Connect();
                }
                catch (OperationCanceledException)
                {
                    this.Close();
                    Application.Exit();
                }
            }
        }

        private void InitializeTextBoxes()
        {
            UsuarioBox.TextChanged += (sender, e) => _service.User = UsuarioBox.Text;
            UsuarioBox.TextChanged += (sender, e) => InitializeConnectList();
            PassUPVBox.TextChanged += (sender, e) => _service.WDrive.Password = PassUPVBox.Text;
            PassDSICBox.TextChanged += (sender, e) => _service.DSICDrive.Password = PassDSICBox.Text;
        }

        private void InitializeConnectList()
        {
            if (_service.VPN_DSIC.Name == null) _service.VPN_DSIC.SetNameAuto();

            listaConectar.Items.Clear();
            listaConectar.Items.Add(new ListItem("Disco W", _service.WDrive));
            listaConectar.Items.Add(new ListItem("Disco DSIC", _service.DSICDrive));
            listaConectar.Items.Add(new ListItem("Portal DSIC", _service.VPN_DSIC));
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            var item = ((ListItem)listaConectar.SelectedItem).Value;
            if (item != null) await ((Connectable)item).ConnectAsync();
        }

        private async void DisconnectButton_Click(object sender, EventArgs e)
        {
            var item = ((ListItem)listaConectar.SelectedItem).Value;
            if (item != null) await ((Connectable)item).DisconnectAsync();
        }

        private void LinuxButton_Click(object sender, EventArgs e)
        {
            RemoteDesktop.ConnectToLinuxDesktop();
        }

        private void WindowsButton_Click(object sender, EventArgs e)
        {
            RemoteDesktop.ConnectToWindowsDesktop();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var item = ((ListItem)listaConectar.SelectedItem).Value;
            if (item != null) ((Openable)item).Open();
        }

    }
}
