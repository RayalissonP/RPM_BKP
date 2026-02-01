using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace RPM_BKP.Scripts
{
    public static class CertificadosExporter
    {
        public static void Exportar(string pastaDestino)
        {
            string storeLocation = "CurrentUser";
            string arquivoSaida = Path.Combine(
                pastaDestino,
                $"Certificados_Pessoal_{storeLocation}.html"
            );

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            if (store.Certificates.Count == 0)
            {
                MostrarAvisoSemCertificados();
                store.Close();
                return;
            }

            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='pt-BR'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Certificados – Aba Pessoal (CurrentUser)</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,sans-serif;background:#f4f4f4;color:#333}");
            html.AppendLine("h2{text-align:center}");
            html.AppendLine("table{border-collapse:collapse;width:90%;margin:auto;background:#fff}");
            html.AppendLine("th,td{border:1px solid #999;padding:8px;text-align:left}");
            html.AppendLine("th{background:#4CAF50;color:white}");
            html.AppendLine("tr:nth-child(even){background:#f2f2f2}");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("<h2>Certificados – Aba Pessoal (CurrentUser)</h2>");
            html.AppendLine($"<p style='text-align:center;'>Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");

            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>Nome do Certificado</th>");
            html.AppendLine("<th>Válido de</th>");
            html.AppendLine("<th>Válido até</th>");
            html.AppendLine("<th>Chave exportável</th>");
            html.AppendLine("</tr>");

            foreach (X509Certificate2 cert in store.Certificates
            .Cast<X509Certificate2>()
            .OrderBy(c => c.NotAfter))
            {
                string nomeCertificado = string.IsNullOrWhiteSpace(cert.FriendlyName)
                    ? cert.Subject
                    : cert.FriendlyName;

                string chaveExportavel = VerificarChaveExportavel(cert);

                html.AppendLine("<tr>");
                html.AppendLine($"<td>{nomeCertificado}</td>");
                html.AppendLine($"<td>{cert.NotBefore:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{cert.NotAfter:dd/MM/yyyy}</td>");
                html.AppendLine($"<td>{chaveExportavel}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            store.Close();

            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);

            Process.Start(new ProcessStartInfo
            {
                FileName = arquivoSaida,
                UseShellExecute = true
            });
        }

        private static string VerificarChaveExportavel(X509Certificate2 cert)
        {
            try
            {
                if (!cert.HasPrivateKey)
                    return "Não";

                // CSP (mais comum em certificados antigos)
                if (cert.PrivateKey is RSACryptoServiceProvider rsaCsp)
                {
                    return rsaCsp.CspKeyContainerInfo.Exportable ? "Sim" : "Não";
                }

                // CNG / outros provedores
                return "Não identificável";
            }
            catch
            {
                return "Não identificável";
            }
        }

        private static void MostrarAvisoSemCertificados()
        {
            int tempo = 5;

            Form form = new Form
            {
                Text = "Aviso",
                Size = new System.Drawing.Size(420, 150),
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label label = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10),
                Text = $"Nenhum certificado foi encontrado na aba Pessoal.\nFechando em {tempo} segundos..."
            };

            form.Controls.Add(label);

            Timer timer = new Timer { Interval = 1000 };

            timer.Tick += (s, e) =>
            {
                tempo--;
                if (tempo <= 0)
                {
                    timer.Stop();
                    form.Close();
                }
                else
                {
                    label.Text = $"Nenhum certificado foi encontrado na aba Pessoal.\nFechando em {tempo} segundos...";
                }
            };

            timer.Start();
            form.ShowDialog();
        }
    }
}
