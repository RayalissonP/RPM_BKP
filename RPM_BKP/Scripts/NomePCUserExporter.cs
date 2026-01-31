using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace RPM_BKP.Scripts
{
    public static class NomePCUserExporter
    {
        public static void Exportar(string pastaDestino)
        {
            string nomePC = Environment.MachineName;
            string arquivoSaida = Path.Combine(pastaDestino, "Usuarios_NomeComputador.html");

            var usuarios = ObterUsuariosComTipo();

            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='pt-BR'>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>Usuários do Computador</title>");
            html.AppendLine("<style>");
            html.AppendLine("body{font-family:Segoe UI,Arial;background:#f4f6f8;padding:20px}");
            html.AppendLine("h1{background:#0078d7;color:#fff;padding:15px;border-radius:6px}");
            html.AppendLine("table{width:100%;border-collapse:collapse;background:#fff;margin-top:20px}");
            html.AppendLine("th,td{padding:10px;border-bottom:1px solid #ddd;text-align:left}");
            html.AppendLine("th{background:#f0f0f0}");
            html.AppendLine(".admin{color:green;font-weight:bold}");
            html.AppendLine(".user{color:#333}");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("<h1>Usuários do Computador</h1>");
            html.AppendLine($"<p><b>Nome do computador:</b> {nomePC}</p>");
            html.AppendLine($"<p><b>Gerado em:</b> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");

            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Usuário</th><th>Tipo</th></tr>");

            foreach (var u in usuarios.OrderBy(x => x.Nome))
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{u.Nome}</td>");
                html.AppendLine(u.IsAdmin
                    ? "<td class='admin'>Administrador</td>"
                    : "<td class='user'>Usuário padrão</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            File.WriteAllText(arquivoSaida, html.ToString(), Encoding.UTF8);
        }

        // =========================================================

        private static readonly string[] ContasSistema =
        {
            "WDAGUtilityAccount",
            "DefaultAccount",
            "Guest",
            "Convidado"
            // "Administrador" // 
        };
        private static List<UsuarioInfo> ObterUsuariosComTipo()
        {
            var lista = new List<UsuarioInfo>();

            using (var ctx = new PrincipalContext(ContextType.Machine))
            {
                var adminGroup = GroupPrincipal.FindByIdentity(
                    ctx,
                    IdentityType.Sid,
                    "S-1-5-32-544"
                );

                var usuarios = new PrincipalSearcher(
                    new UserPrincipal(ctx)
                ).FindAll();

                foreach (UserPrincipal user in usuarios)
                {
                    if (string.IsNullOrWhiteSpace(user.SamAccountName))
                        continue;

                    // 🔥 FILTRO DE CONTAS DE SISTEMA
                    if (ContasSistema.Contains(user.SamAccountName, StringComparer.OrdinalIgnoreCase))
                        continue;

                    bool isAdmin = false;

                    if (adminGroup != null)
                    {
                        isAdmin = user.IsMemberOf(adminGroup);
                    }

                    lista.Add(new UsuarioInfo
                    {
                        Nome = user.SamAccountName,
                        IsAdmin = isAdmin
                    });
                }


                return lista;
            }
        }  

        // =========================================================

        private class UsuarioInfo
        {
            public string Nome;
            public bool IsAdmin;
        }
    }
}
