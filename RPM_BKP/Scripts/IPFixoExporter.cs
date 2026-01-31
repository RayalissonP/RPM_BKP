using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace RPM_BKP.Scripts
{
    public static class IPFixoExporter
    {
        public static void Exportar(string pastaDestino)
        {
            var sb = new StringBuilder();
            bool encontrouIPFixo = false;

            var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE"
            );

            foreach (ManagementObject mo in searcher.Get())
            {
                bool dhcpAtivo = Convert.ToBoolean(mo["DHCPEnabled"]);

                // Se DHCP estiver ativo, não é IP fixo
                if (dhcpAtivo)
                    continue;

                string[] ips = mo["IPAddress"] as string[];
                if (ips == null)
                    continue;

                var ipv4s = ips.Where(ip => ip.Contains(".")).ToList();
                if (!ipv4s.Any())
                    continue;

                encontrouIPFixo = true;

                sb.AppendLine("======================================");
                sb.AppendLine("Adaptador: " + (mo["Description"] ?? "Desconhecido"));
                sb.AppendLine("IPv4 Fixo:");

                foreach (var ip in ipv4s)
                    sb.AppendLine(" - " + ip);

                sb.AppendLine("Máscara: " + string.Join(", ", (mo["IPSubnet"] as string[]) ?? Array.Empty<string>()));
                sb.AppendLine("Gateway: " + string.Join(", ", (mo["DefaultIPGateway"] as string[]) ?? Array.Empty<string>()));
                sb.AppendLine("DNS: " + string.Join(", ", (mo["DNSServerSearchOrder"] as string[]) ?? Array.Empty<string>()));
                sb.AppendLine("======================================");
                sb.AppendLine();
            }

            // 🔥 Se não encontrou IP fixo, não faz nada
            if (!encontrouIPFixo)
                return;

            string arquivoSaida = Path.Combine(pastaDestino, "IPv4_Fixo_Detectado.txt");

            File.WriteAllText(arquivoSaida, sb.ToString(), Encoding.UTF8);
        }
    }
}
