using System;
using System.IO;
using System.Management;
using System.Text;

namespace RPM_BKP.Scripts
{
    public static class ImpressorasExporter
    {
        public static void Exportar(string pastaDestino)
        {
            string arquivoSaida = Path.Combine(pastaDestino, "Impressoras_Instaladas.html");

            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='pt-BR'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Impressoras Instaladas</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,Arial;background:#f5f5f5;padding:20px}");
            html.AppendLine("h1{margin-bottom:20px}");
            html.AppendLine("table{border-collapse:collapse;width:100%;background:#fff}");
            html.AppendLine("th,td{padding:8px 10px;border-bottom:1px solid #ddd;font-size:14px}");
            html.AppendLine("th{background:#f0f0f0;text-align:left}");
            html.AppendLine("tr:hover{background:#f9f9f9}");
            html.AppendLine(".padrao{font-weight:bold;color:#0a5}");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<h1>Impressoras Instaladas</h1>");
            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>Nome</th>");
            html.AppendLine("<th>Driver</th>");
            html.AppendLine("<th>Porta</th>");
            html.AppendLine("<th>Status</th>");
            html.AppendLine("<th>Padrão</th>");
            html.AppendLine("</tr>");

            using (ManagementObjectSearcher searcher =
                   new ManagementObjectSearcher("SELECT * FROM Win32_Printer"))
            {
                foreach (ManagementObject printer in searcher.Get())
                {
                    string nome = printer["Name"]?.ToString();
                    string driver = printer["DriverName"]?.ToString();
                    string porta = printer["PortName"]?.ToString();
                    bool padrao = printer["Default"] != null && (bool)printer["Default"];
                    string status = printer["PrinterStatus"]?.ToString();

                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{nome}</td>");
                    html.AppendLine($"<td>{driver}</td>");
                    html.AppendLine($"<td>{porta}</td>");
                    html.AppendLine($"<td>{StatusAmigavel(status)}</td>");
                    html.AppendLine($"<td class='padrao'>{(padrao ? "Sim" : "")}</td>");
                    html.AppendLine("</tr>");
                }
            }

            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
        }

        private static string StatusAmigavel(string status)
        {
            switch (status)
            {
                case "3": return "Pronta";
                case "4": return "Imprimindo";
                case "5": return "Aquecendo";
                default: return "Desconhecido";
            }
        }
    }
}
