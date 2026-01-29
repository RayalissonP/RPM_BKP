namespace RPM_BKP
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnSelecionarPasta = new System.Windows.Forms.Button();
            this.txtCaminhoPasta = new System.Windows.Forms.TextBox();
            this.chkExportarChromeEdge = new System.Windows.Forms.CheckBox();
            this.chkExportarFirefox = new System.Windows.Forms.CheckBox();
            this.chkGerarListaProgramas = new System.Windows.Forms.CheckBox();
            this.chkGerarListaImpressoras = new System.Windows.Forms.CheckBox();
            this.chkListarCertificados = new System.Windows.Forms.CheckBox();
            this.chkSalvarSerialWindows = new System.Windows.Forms.CheckBox();
            this.chkVerificarIPFixo = new System.Windows.Forms.CheckBox();
            this.btnExecutar = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.NomePC_User = new System.Windows.Forms.CheckBox();
            this.exceptionJava = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSelecionarPasta
            // 
            this.btnSelecionarPasta.Location = new System.Drawing.Point(99, 20);
            this.btnSelecionarPasta.Name = "btnSelecionarPasta";
            this.btnSelecionarPasta.Size = new System.Drawing.Size(180, 23);
            this.btnSelecionarPasta.TabIndex = 0;
            this.btnSelecionarPasta.Text = "Selecionar pasta";
            this.btnSelecionarPasta.Click += new System.EventHandler(this.btnSelecionarPasta_Click);
            // 
            // txtCaminhoPasta
            // 
            this.txtCaminhoPasta.Location = new System.Drawing.Point(27, 49);
            this.txtCaminhoPasta.Name = "txtCaminhoPasta";
            this.txtCaminhoPasta.ReadOnly = true;
            this.txtCaminhoPasta.Size = new System.Drawing.Size(320, 20);
            this.txtCaminhoPasta.TabIndex = 1;
            // 
            // chkExportarChromeEdge
            // 
            this.chkExportarChromeEdge.Location = new System.Drawing.Point(37, 85);
            this.chkExportarChromeEdge.Name = "chkExportarChromeEdge";
            this.chkExportarChromeEdge.Size = new System.Drawing.Size(194, 24);
            this.chkExportarChromeEdge.TabIndex = 2;
            this.chkExportarChromeEdge.Text = "Exportar Favoritos Chrome e Edge";
            this.chkExportarChromeEdge.CheckedChanged += new System.EventHandler(this.chkExportarChromeEdge_CheckedChanged);
            // 
            // chkExportarFirefox
            // 
            this.chkExportarFirefox.Location = new System.Drawing.Point(37, 108);
            this.chkExportarFirefox.Name = "chkExportarFirefox";
            this.chkExportarFirefox.Size = new System.Drawing.Size(185, 24);
            this.chkExportarFirefox.TabIndex = 3;
            this.chkExportarFirefox.Text = "Exportar Favoritos Firefox";
            this.chkExportarFirefox.CheckedChanged += new System.EventHandler(this.chkExportarFirefox_CheckedChanged);
            // 
            // chkGerarListaProgramas
            // 
            this.chkGerarListaProgramas.Location = new System.Drawing.Point(37, 131);
            this.chkGerarListaProgramas.Name = "chkGerarListaProgramas";
            this.chkGerarListaProgramas.Size = new System.Drawing.Size(168, 24);
            this.chkGerarListaProgramas.TabIndex = 4;
            this.chkGerarListaProgramas.Text = "Gerar lista de Programas";
            this.chkGerarListaProgramas.CheckedChanged += new System.EventHandler(this.chkGerarListaProgramas_CheckedChanged);
            // 
            // chkGerarListaImpressoras
            // 
            this.chkGerarListaImpressoras.Location = new System.Drawing.Point(37, 154);
            this.chkGerarListaImpressoras.Name = "chkGerarListaImpressoras";
            this.chkGerarListaImpressoras.Size = new System.Drawing.Size(168, 24);
            this.chkGerarListaImpressoras.TabIndex = 5;
            this.chkGerarListaImpressoras.Text = "Gerar lista de Impressoras";
            this.chkGerarListaImpressoras.CheckedChanged += new System.EventHandler(this.chkGerarListaImpressoras_CheckedChanged);
            // 
            // chkListarCertificados
            // 
            this.chkListarCertificados.Location = new System.Drawing.Point(37, 177);
            this.chkListarCertificados.Name = "chkListarCertificados";
            this.chkListarCertificados.Size = new System.Drawing.Size(168, 24);
            this.chkListarCertificados.TabIndex = 6;
            this.chkListarCertificados.Text = "Listar Certificados";
            this.chkListarCertificados.CheckedChanged += new System.EventHandler(this.chkListarCertificados_CheckedChanged);
            // 
            // chkSalvarSerialWindows
            // 
            this.chkSalvarSerialWindows.Location = new System.Drawing.Point(37, 200);
            this.chkSalvarSerialWindows.Name = "chkSalvarSerialWindows";
            this.chkSalvarSerialWindows.Size = new System.Drawing.Size(144, 24);
            this.chkSalvarSerialWindows.TabIndex = 7;
            this.chkSalvarSerialWindows.Text = "Salvar Serial Windows";
            this.chkSalvarSerialWindows.CheckedChanged += new System.EventHandler(this.chkSalvarSerialWindows_CheckedChanged);
            // 
            // chkVerificarIPFixo
            // 
            this.chkVerificarIPFixo.Location = new System.Drawing.Point(37, 223);
            this.chkVerificarIPFixo.Name = "chkVerificarIPFixo";
            this.chkVerificarIPFixo.Size = new System.Drawing.Size(144, 24);
            this.chkVerificarIPFixo.TabIndex = 8;
            this.chkVerificarIPFixo.Text = "Verificar IP Fixo";
            this.chkVerificarIPFixo.CheckedChanged += new System.EventHandler(this.chkVerificarIPFixo_CheckedChanged);
            // 
            // btnExecutar
            // 
            this.btnExecutar.Location = new System.Drawing.Point(115, 312);
            this.btnExecutar.Name = "btnExecutar";
            this.btnExecutar.Size = new System.Drawing.Size(138, 23);
            this.btnExecutar.TabIndex = 9;
            this.btnExecutar.Text = "Executar";
            this.btnExecutar.Click += new System.EventHandler(this.btnExecutar_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(27, 341);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(320, 23);
            this.progressBar.TabIndex = 10;
            this.progressBar.Click += new System.EventHandler(this.progressBar_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(27, 325);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(320, 13);
            this.lblStatus.TabIndex = 11;
            // 
            // NomePC_User
            // 
            this.NomePC_User.Location = new System.Drawing.Point(37, 246);
            this.NomePC_User.Name = "NomePC_User";
            this.NomePC_User.Size = new System.Drawing.Size(185, 24);
            this.NomePC_User.TabIndex = 12;
            this.NomePC_User.Text = "Salvar Nome de Usuário e PC";
            this.NomePC_User.CheckedChanged += new System.EventHandler(this.NomePC_User_CheckedChanged);
            // 
            // exceptionJava
            // 
            this.exceptionJava.Location = new System.Drawing.Point(37, 270);
            this.exceptionJava.Name = "exceptionJava";
            this.exceptionJava.Size = new System.Drawing.Size(168, 24);
            this.exceptionJava.TabIndex = 13;
            this.exceptionJava.Text = "Salvar Sites Exceções Java";
            this.exceptionJava.CheckedChanged += new System.EventHandler(this.exceptionJava_CheckedChanged);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(384, 386);
            this.Controls.Add(this.exceptionJava);
            this.Controls.Add(this.NomePC_User);
            this.Controls.Add(this.btnSelecionarPasta);
            this.Controls.Add(this.txtCaminhoPasta);
            this.Controls.Add(this.chkExportarChromeEdge);
            this.Controls.Add(this.chkExportarFirefox);
            this.Controls.Add(this.chkGerarListaProgramas);
            this.Controls.Add(this.chkGerarListaImpressoras);
            this.Controls.Add(this.chkListarCertificados);
            this.Controls.Add(this.chkSalvarSerialWindows);
            this.Controls.Add(this.chkVerificarIPFixo);
            this.Controls.Add(this.btnExecutar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RPM BKP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnSelecionarPasta;
        private System.Windows.Forms.TextBox txtCaminhoPasta;
        private System.Windows.Forms.CheckBox chkExportarChromeEdge;
        private System.Windows.Forms.CheckBox chkExportarFirefox;
        private System.Windows.Forms.CheckBox chkGerarListaProgramas;
        private System.Windows.Forms.CheckBox chkGerarListaImpressoras;
        private System.Windows.Forms.CheckBox chkListarCertificados;
        private System.Windows.Forms.CheckBox chkSalvarSerialWindows;
        private System.Windows.Forms.CheckBox chkVerificarIPFixo;
        private System.Windows.Forms.Button btnExecutar;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox NomePC_User;
        private System.Windows.Forms.CheckBox exceptionJava;
    }
}
