using System;
using System.IO;

namespace RPM_BKP.Scripts
{
    public static class JavaExceptionExporter
    {
        public static void Exportar(string pastaDestino)
        {
            // Caminho do arquivo exception.sites do usuário logado
            string caminhoOrigem = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                @"AppData\LocalLow\Sun\Java\Deployment\security\exception.sites"
            );

            // Se não existir, não faz nada
            if (!File.Exists(caminhoOrigem))
                return;

            // Pasta destino: "Sites de exceção Java"
            string pastaJava = Path.Combine(pastaDestino, "Sites de exceção Java");
            Directory.CreateDirectory(pastaJava);

            // Arquivo de destino
            string caminhoDestino = Path.Combine(pastaJava, "exception.sites");

            // Copiar (sobrescreve se já existir)
            File.Copy(caminhoOrigem, caminhoDestino, true);
        }
    }
}
