using System;
using System.IO;
using IWshRuntimeLibrary;

namespace RPM_BKP.Scripts
{
    public static class JavaExceptionExporter
    {
        public static void Exportar(string pastaDestino)
        {
            
            string pastaOrigem = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                @"AppData\LocalLow\Sun\Java\Deployment\security"
            );

            string caminhoOrigem = Path.Combine(pastaOrigem, "exception.sites");

            if (!System.IO.File.Exists(caminhoOrigem))
                return;

            string pastaJava = Path.Combine(pastaDestino, "Sites de exceção Java");
            Directory.CreateDirectory(pastaJava);

            string caminhoDestino = Path.Combine(pastaJava, "exception.sites");
            System.IO.File.Copy(caminhoOrigem, caminhoDestino, true);

            CriarAtalhoParaPastaOrigem(pastaOrigem, pastaJava);
        }

        private static void CriarAtalhoParaPastaOrigem(string pastaOrigem, string pastaDestino)
        {
            string caminhoAtalho = Path.Combine(
                pastaDestino,
                "Atalho para pasta de origem.lnk"
            );

            WshShell shell = new WshShell();
            IWshShortcut atalho = (IWshShortcut)shell.CreateShortcut(caminhoAtalho);

            atalho.TargetPath = pastaOrigem;
            atalho.WorkingDirectory = pastaOrigem;
            atalho.Description = "Pasta original do Java (exception.sites)";
            atalho.IconLocation = @"%SystemRoot%\System32\shell32.dll,4";

            atalho.Save();
        }
    }
}
