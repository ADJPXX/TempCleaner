using System.Diagnostics;
using System.Security.Principal;

namespace TempCleaner;

public static class Program
{
    private static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    
    public static void Main(string[] args)
    {
        if (!IsAdmin())
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule!.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                Console.WriteLine("Permissão de administrador negada.");
            }

            return;
        }
        
        var tempPorcentagem = Path.GetTempPath();
        
        const string temp = "Temp";
        
        var windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        
        var pathTemp = Path.Combine(windows, temp);
        
        var pastaImagens = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        var pastaScreenshots = Path.Combine(pastaImagens, "Screenshots");
        
        LimparArquivos(tempPorcentagem);
        LimparPastas(tempPorcentagem);

        LimparArquivos(pathTemp);
        LimparPastas(pathTemp);
        
        LimparScreenshots(pastaScreenshots);
        
        LimparLixeira();
    }

    
    private static void LimparArquivos(string temp)
    {
        foreach (var file in Directory.GetFiles(temp))
        {
            try
            {
                Console.WriteLine($"ARQUIVO ALVO: {file}");
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
                Console.WriteLine("ARQUIVO DELETADO!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FALHA AO DELETAR O ARQUIVO: {file}");
                Console.WriteLine($"ERRO: {ex.Message}\n");
            }
        }
    }


    private static void LimparPastas(string temp)
    {
        foreach (var file in Directory.GetDirectories(temp))
        {
            try
            {
                Console.WriteLine($"PASTA ALVO: {file}");
                Directory.Delete(file, true);
                Console.WriteLine("PASTA DELETADA!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FALHA AO DELETAR A PASTA: {file}");
                Console.WriteLine($"ERRO: {ex.Message}\n");
            }
        }
    }


    private static void LimparLixeira()
    {
        try
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                var lixeiraDrive = Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = $"/c rd /s /q \"{drive.Name}$RECYCLE.BIN\""
                });

                lixeiraDrive?.WaitForExit();
            }

            var lixeiraWindows = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "/C Clear-RecycleBin -Force"
            });
            
            lixeiraWindows?.WaitForExit();
        }

        catch (Exception ex)
        {
            Console.WriteLine($"FALHA AO DELETAR O ARQUIVO! ERRO: {ex.Message}");
        }
    }

    
    private static void LimparScreenshots(string screenshots)
    {
        try
        {
            var pastaScreenhots = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"/C rd /s /q \"{screenshots}\""
            });
            
            pastaScreenhots?.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FALHA AO DELETAR A PASTA DE SCREENSHOTS. ERRO: {ex.Message}");
        }
    }
}