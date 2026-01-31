using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RPM_BKP.Scripts;


namespace RPM_BKP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private void btnSelecionarPasta_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Selecione a pasta de destino";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtCaminhoPasta.Text = dialog.SelectedPath;
                }
            }
        }

        private async void btnExecutar_Click(object sender, EventArgs e)
        {
            if (!chkExportarChromeEdge.Checked &&
                !chkExportarFirefox.Checked &&
                !chkGerarListaProgramas.Checked &&
                !chkGerarListaImpressoras.Checked &&
                !chkListarCertificados.Checked &&
                !chkSalvarSerialWindows.Checked &&
                !chkVerificarIPFixo.Checked &&
                !NomePC_User.Checked &&
                !exceptionJava.Checked)
            {
                MessageBox.Show("Nenhuma ação foi selecionada!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCaminhoPasta.Text))
            {
                MessageBox.Show("Selecione uma pasta de destino!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            BloquearInterface(true);

            int totalAcoes = ContarAcoesSelecionadas();
            progressBar.Minimum = 0;
            progressBar.Maximum = totalAcoes;
            progressBar.Value = 0;

            try
            {
                if (chkExportarChromeEdge.Checked)
                {
                    AtualizarStatus("Exportando favoritos do Chrome e Edge...");
                    await Task.Run(() => ChromeEdgeExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkExportarFirefox.Checked)
                {
                    AtualizarStatus("Exportando favoritos do Firefox...");
                    await Task.Run(() => FirefoxExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkGerarListaProgramas.Checked)
                {
                    AtualizarStatus("Gerando lista de programas instalados...");
                    await Task.Run(() => ProgramasExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkGerarListaImpressoras.Checked)
                {
                    AtualizarStatus("Gerando lista de impressoras...");
                    await Task.Run(() => ImpressorasExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkListarCertificados.Checked)
                {
                    AtualizarStatus("Listando certificados instalados...");
                    await Task.Run(() => CertificadosExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkSalvarSerialWindows.Checked)
                {
                    AtualizarStatus("Gerando relatório de licença do Windows...");
                    await Task.Run(() => WindowsLicenseExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (chkVerificarIPFixo.Checked)
                {
                    AtualizarStatus("Verificando configuração de IP fixo...");
                    await Task.Run(() => IPFixoExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (NomePC_User.Checked)
                {
                    AtualizarStatus("Salvando Nome do PC e Usuário...");
                    await Task.Run(() => NomePCUserExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                if (exceptionJava.Checked)
                {
                    AtualizarStatus("Salvando exceções de sites do Java...");
                    await Task.Run(() => JavaExceptionExporter.Exportar(txtCaminhoPasta.Text));
                    progressBar.Value++;
                }

                lblStatus.Text = "Concluído!";
                MessageBox.Show(this, "Processo finalizado com sucesso!",
                    "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                BloquearInterface(false);
            }
        }

        private int ContarAcoesSelecionadas()
        {
            int total = 0;
            if (chkExportarChromeEdge.Checked) total++;
            if (chkExportarFirefox.Checked) total++;
            if (chkGerarListaProgramas.Checked) total++;
            if (chkGerarListaImpressoras.Checked) total++;
            if (chkListarCertificados.Checked) total++;
            if (chkSalvarSerialWindows.Checked) total++;
            if (chkVerificarIPFixo.Checked) total++;
            if (NomePC_User.Checked) total++;
            if (exceptionJava.Checked) total++;
            {
                
            }
            return total;
        }

        private void AtualizarStatus(string texto)
        {
            lblStatus.Text = texto;
            Application.DoEvents();
        }

        private void BloquearInterface(bool bloquear)
        {
            btnExecutar.Enabled = !bloquear;
            btnSelecionarPasta.Enabled = !bloquear;
            btnLimpar.Enabled = !bloquear;
            btnSelecTodos.Enabled = !bloquear;

            foreach (Control c in Controls)
            {
                if (c is CheckBox)
                    c.Enabled = !bloquear;
            }

            if (bloquear)
            {
                progressBar.Value = 0;
                lblStatus.Text = "Iniciando...";
            }
        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }

        private void chkExportarChromeEdge_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkExportarFirefox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkGerarListaProgramas_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkGerarListaImpressoras_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkListarCertificados_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkSalvarSerialWindows_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkVerificarIPFixo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NomePC_User_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void exceptionJava_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnSelecTodos_Click(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                if (c is CheckBox chk)
                {
                    chk.Checked = true;
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                if (c is CheckBox chk)
                {
                    chk.Checked = false;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
 
        }

    }
}
