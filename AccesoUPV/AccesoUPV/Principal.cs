using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AccesoUPV.Library.Connectors;
using AccesoUPV.Library.Services;
using AccesoUPV.Library.Static;

namespace AccesoUPV.GUI
{
    public partial class Principal : Form
    {
        private AccesoUPVService _service;

        public Principal()
        {
            InitializeComponent();
        }

        public Principal(AccesoUPVService service) : this()
        {
            _service = service;
            listaConectar.Items.Add(_service.WDrive);
            listaConectar.Items.Add(_service.DSICDrive);
            listaConectar.Items.Add(_service.VPN_DSIC);
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            var item = listaConectar.SelectedItem;
            if (item is Connectable)
            {
                await ((Connectable) listaConectar.SelectedItem).ConnectAsync();
            }
            else
            {
                throw new ArgumentException("The item is not Connectable");
            }
        }

        private async void disconnectButton_Click(object sender, EventArgs e)
        {
            var item = listaConectar.SelectedItem;
            if (item is Connectable)
            {
                await ((Connectable) listaConectar.SelectedItem).DisconnectAsync();
            }
            else
            {
                throw new ArgumentException("The item is not Connectable");
            }
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
