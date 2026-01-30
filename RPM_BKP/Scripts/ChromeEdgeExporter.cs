using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
namespace RPM_BKP.Scripts
{
    public static class ChromeEdgeExporter
    {
        public static void Exportar(string destino)
        {
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
        }

        private static void ExportarNavegador(string pastaUserData, string pastaDestino, string nomeNavegador)
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
                    $"Favoritos_{nomeNavegador}_{nomePerfil}.html".Replace(" ", "_")
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
                        ProcessarItens(children, html);
                }

                html.AppendLine("</DL><p>");
                File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
            }
        }

        private static void ProcessarItens(JToken itens, StringBuilder html)
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
                        ProcessarItens(item["children"], html);

                    html.AppendLine("</DL><p>");
                }
                else if (tipo == "url")
                {
                    string url = item["url"]?.ToString();
                    html.AppendLine($"<DT><A HREF=\"{url}\">{nome}</A>");
                }
            }
        }
    }
}
