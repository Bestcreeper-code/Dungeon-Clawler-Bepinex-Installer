using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace DungeonClawlerBepinexInstaller
{
    internal class BepinexInstall
    {
        const string BepInExDownloadUrl = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.3/BepInEx_win_x64_5.4.23.3.zip";
        public static string DCPath;
        public static void InstallBepInEx()
        {
            DCPath = Program.DCPath;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Starting BepInEx installation...");
            
            string bepInExZip = Utils.Download(BepInExDownloadUrl,DCPath);
            if (bepInExZip == "X")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;                
                Console.WriteLine($"File already exists at {BepInExDownloadUrl}");
                Thread.Sleep(1000);
                return;
            } 
            if (string.IsNullOrEmpty(bepInExZip) || !File.Exists(bepInExZip))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to download BepInEx. Exiting.");
                return;
            }

            Console.ResetColor();
            try
            {
                Utils.ExtractZip(bepInExZip, DCPath);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error while Extracting the file, possible duplicates");
            }
            Console.WriteLine("BepInEx installation completed successfully. Launching DungeonClawler.exe for initialization");
            if (File.Exists(Path.Combine(DCPath, "DungeonClawler.exe")))
            {
                Process game = Process.Start(Path.Combine(DCPath, "DungeonClawler.exe"));
                Thread.Sleep(3000);
                game.Close();
            }
        }
    }
}
