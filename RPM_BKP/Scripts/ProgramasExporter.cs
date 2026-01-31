using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RPM_BKP.Scripts
{
    public static class ProgramasExporter
    {
        public static void Exportar(string pastaDestino)
        {
            string pastaIcons = Path.Combine(pastaDestino, "icons");
            Directory.CreateDirectory(pastaIcons);

            // torna a pasta oculta
            File.SetAttributes(pastaIcons, File.GetAttributes(pastaIcons) | FileAttributes.Hidden);

            var programas = LerProgramasInstalados();

            var html = new StringBuilder();
            html.AppendLine("<html><head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI;background:#f4f4f4}");
            html.AppendLine("table{border-collapse:collapse;width:100%;background:#fff}");
            html.AppendLine("th,td{padding:8px;border-bottom:1px solid #ddd;text-align:left}");
            html.AppendLine("th{background:#eee}");
            html.AppendLine("img{width:24px;height:24px}");
            html.AppendLine("</style></head><body>");
            html.AppendLine("<h2>Programas Instalados</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th></th><th>Nome</th><th>Editor</th><th>Instalado em</th><th>Tamanho</th><th>Versão</th></tr>");

            foreach (var p in programas.OrderBy(x => x.Nome))
            {
                string iconFile = Path.Combine(pastaIcons, LimparNomeArquivo(p.Nome) + ".png");

                if (!File.Exists(iconFile))
                    SalvarIcone(p, iconFile);

                html.AppendLine("<tr>");
                html.AppendLine($"<td><img src='icons/{Path.GetFileName(iconFile)}'></td>");
                html.AppendLine($"<td>{p.Nome}</td>");
                html.AppendLine($"<td>{p.Editor}</td>");
                html.AppendLine($"<td>{p.DataInstalacao}</td>");
                html.AppendLine($"<td>{p.Tamanho}</td>");
                html.AppendLine($"<td>{p.Versao}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table></body></html>");

            string arquivoSaida = Path.Combine(pastaDestino, "Programas_Instalados.html");
            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
            Process.Start(new ProcessStartInfo
            {
                FileName = arquivoSaida,
                UseShellExecute = true
            });
        }

        private static List<Programa> LerProgramasInstalados()
        {
            var lista = new List<Programa>();

            string[] caminhos =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var caminho in caminhos)
            {
                using (var baseKey = Registry.LocalMachine.OpenSubKey(caminho))
                {
                    if (baseKey == null) continue;

                    foreach (var subKeyName in baseKey.GetSubKeyNames())
                    {
                        using (var subKey = baseKey.OpenSubKey(subKeyName))
                        {
                            if (subKey == null) continue;

                            string nome = subKey.GetValue("DisplayName") as string;
                            if (string.IsNullOrWhiteSpace(nome))
                                continue;

                            // FILTROS IGUAIS AO PAINEL DE CONTROLE
                            if (subKey.GetValue("SystemComponent")?.ToString() == "1")
                                continue;
                            if (subKey.GetValue("ReleaseType") != null)
                                continue;
                            if (subKey.GetValue("ParentKeyName") != null)
                                continue;

                            lista.Add(new Programa
                            {
                                Nome = nome,
                                Editor = subKey.GetValue("Publisher") as string ?? "",
                                Versao = subKey.GetValue("DisplayVersion") as string ?? "",
                                DataInstalacao = FormatarData(subKey.GetValue("InstallDate") as string),
                                Tamanho = FormatarTamanho(subKey.GetValue("EstimatedSize")),
                                DisplayIcon = subKey.GetValue("DisplayIcon") as string,
                                InstallLocation = subKey.GetValue("InstallLocation") as string,
                                UninstallString = subKey.GetValue("UninstallString") as string
                            });
                        }
                    }
                }
            }

            return lista;
        }

        private static void SalvarIcone(Programa p, string destino)
        {
            try
            {
                string caminhoExe = ObterExecutavel(p);
                if (string.IsNullOrEmpty(caminhoExe) || !File.Exists(caminhoExe))
                {
                    CriarIconePadrao(destino);
                    return;
                }

                Icon icon = Icon.ExtractAssociatedIcon(caminhoExe);
                if (icon == null)
                {
                    CriarIconePadrao(destino);
                    return;
                }

                using (Bitmap bmp = icon.ToBitmap())
                {
                    bmp.Save(destino);
                }
            }
            catch
            {
                CriarIconePadrao(destino);
            }
        }

        private static string ObterExecutavel(Programa p)
        {
            if (!string.IsNullOrWhiteSpace(p.DisplayIcon))
                return LimparCaminho(p.DisplayIcon);

            if (!string.IsNullOrWhiteSpace(p.InstallLocation) && Directory.Exists(p.InstallLocation))
            {
                var exe = Directory.GetFiles(p.InstallLocation, "*.exe").FirstOrDefault();
                if (exe != null) return exe;
            }

            if (!string.IsNullOrWhiteSpace(p.UninstallString))
                return LimparCaminho(p.UninstallString);

            return null;
        }

        private static void CriarIconePadrao(string destino)
        {
            using (Bitmap bmp = new Bitmap(32, 32))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.Gray, 0, 0, 31, 31);
                bmp.Save(destino);
            }
        }

        private static string LimparCaminho(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return null;
            valor = valor.Replace("\"", "");
            int idx = valor.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
            if (idx > 0)
                return valor.Substring(0, idx + 4);
            return valor;
        }

        private static string LimparNomeArquivo(string nome)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                nome = nome.Replace(c, '_');
            return nome;
        }

        private static string FormatarData(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 8)
                return "";
            return $"{data.Substring(6, 2)}/{data.Substring(4, 2)}/{data.Substring(0, 4)}";
        }

        private static string FormatarTamanho(object valor)
        {
            if (valor == null) return "";
            double kb = Convert.ToDouble(valor);
            if (kb > 1024)
                return $"{kb / 1024:0.##} MB";
            return $"{kb:0.##} KB";
        }

        // =========================================================

     private class Programa
        {
            public string Nome;
            public string Editor;
            public string Versao;
            public string DataInstalacao;
            public string Tamanho;
            public string DisplayIcon;
            public string InstallLocation;
            public string UninstallString;
        }
    }
}
