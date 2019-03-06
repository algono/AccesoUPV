using System;
using System.Windows.Forms;
using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;

namespace AccesoUPV.GUI
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
            _service.VPN_UPV.Name = "UPV"; // TEST CODE
            if (!_service.VPN_UPV.IsReachable()) _service.VPN_UPV.Connect();
            InitializeConnectList();
        }

        private void InitializeConnectList()
        {
            // Testing code. Delete before production.
            _service.WDrive.UserName = "algono";
            _service.VPN_DSIC.SetNameAuto();

            listaConectar.Items.Add(new ListItem("Disco W", _service.WDrive));
            listaConectar.Items.Add(new ListItem("Disco DSIC", _service.DSICDrive));
            listaConectar.Items.Add(new ListItem("Portal DSIC", _service.VPN_DSIC));
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            var item = ((ListItem) listaConectar.SelectedItem).Value;
            if (item != null) await ((Connectable) item).ConnectAsync();
        }

        private async void disconnectButton_Click(object sender, EventArgs e)
        {
            var item = ((ListItem)listaConectar.SelectedItem).Value;
            if (item != null) await ((Connectable) item).DisconnectAsync();
        }

        private void linuxButton_Click(object sender, EventArgs e)
        {
            RemoteDesktop.ConnectToLinuxDesktop();
        }

        private void windowsButton_Click(object sender, EventArgs e)
        {
            RemoteDesktop.ConnectToWindowsDesktop();
        }
    }
}
