using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RPM_BKP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnExecutar_Click(object sender, EventArgs e)
        {
            if (!chkExportarChromeEdge.Checked &&
                !chkExportarFirefox.Checked &&
                !chkGerarListaProgramas.Checked &&
                !chkGerarListaImpressoras.Checked &&
                !chkListarCertificados.Checked &&
                !chkSalvarSerialWindows.Checked &&
                !chkVerificarIPFixo.Checked)
            {
                MessageBox.Show("Nenhuma ação foi selecionada!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtCaminhoPasta.Text))
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
                    await ExportarChromeEdge();
                }

                if (chkExportarFirefox.Checked)
                {
                    AtualizarStatus("Exportando favoritos do Firefox...");
                    await ExportarFirefox();
                }

                lblStatus.Text = "Concluído!";
                MessageBox.Show("Processo finalizado com sucesso!",
                    "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
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

        // =============== EXPORTAR CHROME E EDGE ==================
  
        private async Task ExportarChromeEdge()
        {
            await Task.Run(() =>
            {
                string destino = txtCaminhoPasta.Text;

                ExportarNavegador(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Google\Chrome\User Data"),
                destino,
                "Chrome"
                );

                ExportarNavegador(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Edge\User Data"),
                destino,
                "Edge"
                );

            });

            progressBar.Invoke((MethodInvoker)(() => progressBar.Value++));
        }

        private void ExportarNavegador(string pastaUserData, string pastaDestino, string nomeNavegador)
        {
            if (!Directory.Exists(pastaUserData))
                return;

            foreach (string perfil in Directory.GetDirectories(pastaUserData))
            {
                string bookmarksPath = Path.Combine(perfil, "Bookmarks");
                if (!File.Exists(bookmarksPath))
                    continue;

                string nomePerfil = Path.GetFileName(perfil);
                string arquivoSaida = Path.Combine(
                    pastaDestino,
                    $"Favoritos_{nomeNavegador}_{nomePerfil}.html"
                        .Replace(" ", "_")
                );

                StringBuilder html = new StringBuilder();

                html.AppendLine("<!DOCTYPE NETSCAPE-Bookmark-file-1>");
                html.AppendLine("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
                html.AppendLine("<TITLE>Bookmarks</TITLE>");
                html.AppendLine("<H1>Bookmarks</H1>");
                html.AppendLine($"<H2>{nomeNavegador} - Perfil: {nomePerfil}</H2>");
                html.AppendLine("<DL><p>");

                JObject json = JObject.Parse(File.ReadAllText(bookmarksPath));
                JObject roots = (JObject)json["roots"];

                foreach (var root in roots.Properties())
                {
                    JToken children = root.Value["children"];
                    if (children != null)
                    {
                        ProcessarItens(children, html);
                    }
                }

                html.AppendLine("</DL><p>");

                File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
            }
        }

        // Exportar Favoritos - Firefox
        private async Task ExportarFirefox()
        {
            await Task.Run(() =>
            {
                string pastaPerfis = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Mozilla\Firefox\Profiles"
                );

                if (!Directory.Exists(pastaPerfis))
                    return;

                string destino = txtCaminhoPasta.Text;

                foreach (string perfil in Directory.GetDirectories(pastaPerfis))
                {
                    string placesPath = Path.Combine(perfil, "places.sqlite");
                    if (!File.Exists(placesPath))
                        continue;

                    string nomePerfil = Path.GetFileName(perfil);
                    string arquivoSaida = Path.Combine(
                        destino,
                        $"Favoritos_Firefox_{nomePerfil}.html".Replace(" ", "_")
                    );

                    ExportarPerfilFirefox(placesPath, arquivoSaida, nomePerfil);
                }
            });

            progressBar.Invoke((MethodInvoker)(() => progressBar.Value++));
        }

        private void ExportarPerfilFirefox(string placesOriginal, string arquivoSaida, string nomePerfil)
        {
            string tempDb = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sqlite");
            File.Copy(placesOriginal, tempDb, true);

            var itens = new Dictionary<long, List<BookmarkItem>>();

            using (var conn = new SqliteConnection($"Data Source={tempDb};Mode=ReadOnly"))
            {
                conn.Open();

                string sql = @"
            SELECT b.id, b.parent, b.type, b.title, p.url, b.position
            FROM moz_bookmarks b
            LEFT JOIN moz_places p ON b.fk = p.id
            WHERE b.type IN (1, 2)
            ORDER BY b.parent, b.position
        ";

                using (var cmd = new SqliteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new BookmarkItem
                        {
                            Id = reader.GetInt64(0),
                            Parent = reader.GetInt64(1),
                            Type = reader.GetInt32(2),
                            Title = reader["title"]?.ToString(),
                            Url = reader["url"]?.ToString()
                        };

                        if (!itens.ContainsKey(item.Parent))
                            itens[item.Parent] = new List<BookmarkItem>();

                        itens[item.Parent].Add(item);
                    }
                }
            }

            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE NETSCAPE-Bookmark-file-1>");
            html.AppendLine("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
            html.AppendLine("<TITLE>Bookmarks</TITLE>");
            html.AppendLine("<H1>Bookmarks</H1>");
            html.AppendLine($"<H2>Firefox - Perfil: {nomePerfil}</H2>");
            html.AppendLine("<DL><p>");

            var rootsPermitidos = new Dictionary<string, string>
{
            { "menu", "Menu de favoritos" },
            { "toolbar", "Barra de favoritos" },
            { "unfiled", "Outros favoritos" }
        };

            if (itens.ContainsKey(1))
            {
                foreach (var root in itens[1])
                {
                    if (rootsPermitidos.TryGetValue(root.Title, out string nomeAmigavel))
                    {
                        root.Title = nomeAmigavel;
                        RenderizarItemFirefox(root, itens, html);
                    }
                }
            }


            html.AppendLine("</DL><p>");

            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    File.Delete(tempDb);
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        private void RenderizarItemFirefox(
        BookmarkItem item,
        Dictionary<long, List<BookmarkItem>> itens,
        StringBuilder html)
        {
            if (item.Type == 2) // pasta
            {
                html.AppendLine($"<DT><H3>{item.Title}</H3>");
                html.AppendLine("<DL><p>");

                if (itens.ContainsKey(item.Id))
                {
                    foreach (var filho in itens[item.Id])
                    {
                        RenderizarItemFirefox(filho, itens, html);
                    }
                }

                html.AppendLine("</DL><p>");
            }
            else if (item.Type == 1 && !string.IsNullOrEmpty(item.Url))
            {
                html.AppendLine($"<DT><A HREF=\"{item.Url}\">{item.Title}</A>");
            }
        }


        // Fim Exportar Favoritos - Firefox

        private void ProcessarItens(JToken itens, StringBuilder html)
        {
            foreach (JToken item in itens)
            {
                string tipo = item["type"]?.ToString();
                string nome = item["name"]?.ToString();

                if (tipo == "folder")
                {
                    html.AppendLine($"<DT><H3>{nome}</H3>");
                    html.AppendLine("<DL><p>");

                    if (item["children"] != null)
                    {
                        ProcessarItens(item["children"], html);
                    }

                    html.AppendLine("</DL><p>");
                }
                else if (tipo == "url")
                {
                    string url = item["url"]?.ToString();
                    html.AppendLine($"<DT><A HREF=\"{url}\">{nome}</A>");
                }
            }
        }

        class BookmarkItem
        {
            public long Id { get; set; }
            public long Parent { get; set; }
            public int Type { get; set; } // 1 = URL, 2 = Pasta
            public string Title { get; set; }
            public string Url { get; set; }
        }


        // =========================================================

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
    }
}
