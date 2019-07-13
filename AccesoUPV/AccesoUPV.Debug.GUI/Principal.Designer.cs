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
            this.disconnectButton = new System.Windows.Forms.Button();
            this.linuxButton = new System.Windows.Forms.Button();
            this.windowsButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ajustesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AyudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.índiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.listaConectar = new System.Windows.Forms.ListBox();
            this.openButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(81, 121);
            this.disconnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(76, 19);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Desconectar";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // linuxButton
            // 
            this.linuxButton.Location = new System.Drawing.Point(179, 84);
            this.linuxButton.Margin = new System.Windows.Forms.Padding(2);
            this.linuxButton.Name = "linuxButton";
            this.linuxButton.Size = new System.Drawing.Size(80, 19);
            this.linuxButton.TabIndex = 2;
            this.linuxButton.Text = "Linux";
            this.linuxButton.UseVisualStyleBackColor = true;
            this.linuxButton.Click += new System.EventHandler(this.linuxButton_Click);
            // 
            // windowsButton
            // 
            this.windowsButton.Location = new System.Drawing.Point(179, 107);
            this.windowsButton.Margin = new System.Windows.Forms.Padding(2);
            this.windowsButton.Name = "windowsButton";
            this.windowsButton.Size = new System.Drawing.Size(80, 19);
            this.windowsButton.TabIndex = 3;
            this.windowsButton.Text = "Windows";
            this.windowsButton.UseVisualStyleBackColor = true;
            this.windowsButton.Click += new System.EventHandler(this.windowsButton_Click);
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
            this.ajustesToolStripMenuItem.Name = "ajustesToolStripMenuItem";
            this.ajustesToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.ajustesToolStripMenuItem.Text = "Ajustes";
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
            this.índiceToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.índiceToolStripMenuItem.Text = "Índice";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(9, 121);
            this.connectButton.Margin = new System.Windows.Forms.Padding(2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(68, 19);
            this.connectButton.TabIndex = 8;
            this.connectButton.Text = "Conectar";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 56);
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
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(9, 145);
            this.openButton.Margin = new System.Windows.Forms.Padding(2);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(148, 19);
            this.openButton.TabIndex = 11;
            this.openButton.Text = "Acceder";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 183);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.listaConectar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.windowsButton);
            this.Controls.Add(this.linuxButton);
            this.Controls.Add(this.disconnectButton);
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
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button linuxButton;
        private System.Windows.Forms.Button windowsButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem AyudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem índiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajustesToolStripMenuItem;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listaConectar;
        private System.Windows.Forms.Button openButton;
    }
}

