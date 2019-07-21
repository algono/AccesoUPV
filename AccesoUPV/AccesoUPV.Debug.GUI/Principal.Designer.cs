namespace AccesoUPV.Debug.GUI
{
    partial class Principal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.LinuxButton = new System.Windows.Forms.Button();
            this.WindowsButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ajustesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UsuarioBox = new System.Windows.Forms.ToolStripTextBox();
            this.passwordUPVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PassUPVBox = new System.Windows.Forms.ToolStripTextBox();
            this.passwordDSICToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PassDSICBox = new System.Windows.Forms.ToolStripTextBox();
            this.AyudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.índiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listaConectar = new System.Windows.Forms.ListBox();
            this.OpenButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(81, 121);
            this.DisconnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(76, 19);
            this.DisconnectButton.TabIndex = 1;
            this.DisconnectButton.Text = "Desconectar";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // LinuxButton
            // 
            this.LinuxButton.Location = new System.Drawing.Point(181, 93);
            this.LinuxButton.Margin = new System.Windows.Forms.Padding(2);
            this.LinuxButton.Name = "LinuxButton";
            this.LinuxButton.Size = new System.Drawing.Size(80, 19);
            this.LinuxButton.TabIndex = 2;
            this.LinuxButton.Text = "Linux";
            this.LinuxButton.UseVisualStyleBackColor = true;
            this.LinuxButton.Click += new System.EventHandler(this.LinuxButton_Click);
            // 
            // WindowsButton
            // 
            this.WindowsButton.Location = new System.Drawing.Point(181, 116);
            this.WindowsButton.Margin = new System.Windows.Forms.Padding(2);
            this.WindowsButton.Name = "WindowsButton";
            this.WindowsButton.Size = new System.Drawing.Size(80, 19);
            this.WindowsButton.TabIndex = 3;
            this.WindowsButton.Text = "Windows";
            this.WindowsButton.UseVisualStyleBackColor = true;
            this.WindowsButton.Click += new System.EventHandler(this.WindowsButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ajustesToolStripMenuItem,
            this.AyudaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(285, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ajustesToolStripMenuItem
            // 
            this.ajustesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usuarioToolStripMenuItem,
            this.passwordUPVToolStripMenuItem,
            this.passwordDSICToolStripMenuItem});
            this.ajustesToolStripMenuItem.Name = "ajustesToolStripMenuItem";
            this.ajustesToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.ajustesToolStripMenuItem.Text = "Ajustes";
            // 
            // usuarioToolStripMenuItem
            // 
            this.usuarioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UsuarioBox});
            this.usuarioToolStripMenuItem.Name = "usuarioToolStripMenuItem";
            this.usuarioToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.usuarioToolStripMenuItem.Text = "Usuario";
            // 
            // UsuarioBox
            // 
            this.UsuarioBox.Name = "UsuarioBox";
            this.UsuarioBox.Size = new System.Drawing.Size(100, 23);
            // 
            // passwordUPVToolStripMenuItem
            // 
            this.passwordUPVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PassUPVBox});
            this.passwordUPVToolStripMenuItem.Name = "passwordUPVToolStripMenuItem";
            this.passwordUPVToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.passwordUPVToolStripMenuItem.Text = "Password UPV";
            // 
            // PassUPVBox
            // 
            this.PassUPVBox.Name = "PassUPVBox";
            this.PassUPVBox.Size = new System.Drawing.Size(100, 23);
            // 
            // passwordDSICToolStripMenuItem
            // 
            this.passwordDSICToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PassDSICBox});
            this.passwordDSICToolStripMenuItem.Name = "passwordDSICToolStripMenuItem";
            this.passwordDSICToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.passwordDSICToolStripMenuItem.Text = "Password DSIC";
            // 
            // PassDSICBox
            // 
            this.PassDSICBox.Name = "PassDSICBox";
            this.PassDSICBox.Size = new System.Drawing.Size(100, 23);
            // 
            // AyudaToolStripMenuItem
            // 
            this.AyudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.índiceToolStripMenuItem});
            this.AyudaToolStripMenuItem.Name = "AyudaToolStripMenuItem";
            this.AyudaToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.AyudaToolStripMenuItem.Text = "Ayuda";
            // 
            // índiceToolStripMenuItem
            // 
            this.índiceToolStripMenuItem.Name = "índiceToolStripMenuItem";
            this.índiceToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.índiceToolStripMenuItem.Text = "Índice";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(9, 121);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(68, 19);
            this.ConnectButton.TabIndex = 8;
            this.ConnectButton.Text = "Conectar";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(178, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Escritorios DSIC";
            // 
            // listaConectar
            // 
            this.listaConectar.FormattingEnabled = true;
            this.listaConectar.Location = new System.Drawing.Point(9, 39);
            this.listaConectar.Margin = new System.Windows.Forms.Padding(2);
            this.listaConectar.Name = "listaConectar";
            this.listaConectar.Size = new System.Drawing.Size(148, 69);
            this.listaConectar.TabIndex = 10;
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(9, 145);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(148, 19);
            this.OpenButton.TabIndex = 11;
            this.OpenButton.Text = "Acceder";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 183);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.listaConectar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.WindowsButton);
            this.Controls.Add(this.LinuxButton);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Principal";
            this.Text = "Acceso UPV";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Button LinuxButton;
        private System.Windows.Forms.Button WindowsButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem AyudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem índiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajustesToolStripMenuItem;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listaConectar;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.ToolStripMenuItem usuarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox UsuarioBox;
        private System.Windows.Forms.ToolStripMenuItem passwordUPVToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox PassUPVBox;
        private System.Windows.Forms.ToolStripMenuItem passwordDSICToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox PassDSICBox;
    }
}

