using Microsoft.Data.Sqlite;
using RPM_BKP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RPM_BKP.Scripts
{
    public static class FirefoxExporter
    {
        public static void Exportar(string destino)
        {
            string pastaPerfis = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Mozilla\Firefox\Profiles"
            );

            if (!Directory.Exists(pastaPerfis))
                return;

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
        }

        private static void ExportarPerfilFirefox(string placesOriginal, string arquivoSaida, string nomePerfil)
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
                ORDER BY b.parent, b.position";

                using (var cmd = new SqliteCommand(sql, conn))
                {
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

        private static void RenderizarItemFirefox(
            BookmarkItem item,
            Dictionary<long, List<BookmarkItem>> itens,
            StringBuilder html)
        {
            if (item.Type == 2)
            {
                html.AppendLine($"<DT><H3>{item.Title}</H3>");
                html.AppendLine("<DL><p>");

                if (itens.ContainsKey(item.Id))
                {
                    foreach (var filho in itens[item.Id])
                        RenderizarItemFirefox(filho, itens, html);
                }

                html.AppendLine("</DL><p>");
            }
            else if (item.Type == 1 && !string.IsNullOrEmpty(item.Url))
            {
                html.AppendLine($"<DT><A HREF=\"{item.Url}\">{item.Title}</A>");
            }
        }
    }
}
