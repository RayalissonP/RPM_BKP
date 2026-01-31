using System;
using System.IO;
using System.Management;
using System.Text;

namespace RPM_BKP.Scripts
{
    public static class WindowsLicenseExporter
    {
        public static void Exportar(string pastaDestino)
        {
            string arquivoSaida = Path.Combine(pastaDestino, "Licenca_Windows.html");

            var os = new ManagementObjectSearcher(
                "SELECT Caption, Version, BuildNumber, InstallDate FROM Win32_OperatingSystem")
                .Get()
                .GetEnumerator();

            os.MoveNext();
            var so = os.Current;

            string windowsEdition = so["Caption"]?.ToString();
            string version = so["Version"]?.ToString();
            string build = so["BuildNumber"]?.ToString();

            string installDate;
            try
            {
                installDate = ManagementDateTimeConverter
                    .ToDateTime(so["InstallDate"].ToString())
                    .ToString("dd/MM/yyyy HH:mm");
            }
            catch
            {
                installDate = "Não disponível";
            }

            string activationStatus = "Não ativado ou em avaliação";
            string partialKey = "Não disponível";

            var licenseSearcher = new ManagementObjectSearcher(
                "SELECT * FROM SoftwareLicensingProduct WHERE ApplicationID='55c92734-d682-4d71-983e-d6ec3f16059f'");

            foreach (ManagementObject lic in licenseSearcher.Get())
            {
                if (lic["PartialProductKey"] != null)
                {
                    partialKey = lic["PartialProductKey"].ToString();
                    if (Convert.ToInt32(lic["LicenseStatus"]) == 1)
                        activationStatus = "Licenciado permanentemente";
                    break;
                }
            }

            string oemKey = "Não existe chave OEM no firmware";

            var serviceSearcher = new ManagementObjectSearcher(
                "SELECT OA3xOriginalProductKey FROM SoftwareLicensingService");

            foreach (ManagementObject svc in serviceSearcher.Get())
            {
                if (svc["OA3xOriginalProductKey"] != null &&
                    !string.IsNullOrWhiteSpace(svc["OA3xOriginalProductKey"].ToString()))
                {
                    oemKey = svc["OA3xOriginalProductKey"].ToString();
                }
            }

            string licenseType = "Indeterminada";
            if (!oemKey.StartsWith("Não existe"))
                licenseType = "OEM (BIOS / UEFI)";
            else if (activationStatus.StartsWith("Licenciado"))
                licenseType = "Licença Digital / MAK / KMS";

            var genericKeys = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Home", "TX9XD-98N7V-6WMQ6-BX7FG-H8Q99" },
                { "Home N", "3KHY7-WNT83-DGQKR-F7HPR-844BM" },
                { "Pro", "VK7JG-NPHTM-C97JM-9MPGT-3V66T" },
                { "Pro N", "2B87N-8KFHP-DKV6R-Y2C8J-PKCKT" },
                { "Education", "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2" },
                { "Enterprise", "NPPR9-FWDCX-D2C8J-H872K-2YT43" }
            };

            string rebuiltKey = "Não disponível";

            if (partialKey != "Não disponível")
            {
                foreach (var k in genericKeys)
                {
                    if (windowsEdition.Contains(k.Key))
                    {
                        rebuiltKey = k.Value.Substring(0, k.Value.Length - 5) + partialKey;
                        break;
                    }
                }
            }

            string reportDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='pt-BR'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Relatório de Licença do Windows</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,Arial;background:#f4f6f8;padding:20px}");
            html.AppendLine("h1{background:#0078d7;color:white;padding:15px;border-radius:6px}");
            html.AppendLine("table{width:100%;border-collapse:collapse;background:white;margin-top:20px}");
            html.AppendLine("th,td{padding:12px;border-bottom:1px solid #ddd}");
            html.AppendLine("th{background:#f0f0f0;width:35%}");
            html.AppendLine(".ok{color:green;font-weight:bold}");
            html.AppendLine(".nok{color:red;font-weight:bold}");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("<h1>Relatório de Licença do Windows</h1>");
            html.AppendLine("<table>");
            html.AppendLine($"<tr><th>Windows</th><td>{windowsEdition}</td></tr>");
            html.AppendLine($"<tr><th>Versão</th><td>{version}</td></tr>");
            html.AppendLine($"<tr><th>Build</th><td>{build}</td></tr>");
            html.AppendLine($"<tr><th>Data de instalação</th><td>{installDate}</td></tr>");
            html.AppendLine($"<tr><th>Status de ativação</th><td class='{(activationStatus.StartsWith("Licenciado") ? "ok" : "nok")}'>{activationStatus}</td></tr>");
            html.AppendLine($"<tr><th>Tipo de licença</th><td>{licenseType}</td></tr>");
            html.AppendLine($"<tr><th>Chave OEM real</th><td>{oemKey}</td></tr>");
            html.AppendLine($"<tr><th>Últimos 5 dígitos reais</th><td>{partialKey}</td></tr>");
            html.AppendLine($"<tr><th>Chave reconstruída (genérica)</th><td>{rebuiltKey}</td></tr>");
            html.AppendLine("</table>");

            html.AppendLine("<p style='margin-top:15px;font-size:13px'>");
            html.AppendLine("<b>Observação:</b> a chave reconstruída é apenas para inventário e auditoria.");
            html.AppendLine("</p>");

            html.AppendLine($"<p style='font-size:12px;color:#666'>Relatório gerado em {reportDate}</p>");
            html.AppendLine("</body></html>");

            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
        }
    }
}
