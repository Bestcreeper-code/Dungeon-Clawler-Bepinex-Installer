using System;
using System.IO;
using System.Windows.Forms;

namespace DungeonClawlerBepinexInstaller
{
    internal class Program
    {
        public static string DCPath;
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Bepinex Installer for Dungeon Clawler";
            DCPath = Path.GetPathRoot(Environment.CurrentDirectory) + "Program Files (x86)\\Steam\\steamapps\\common\\Dungeon Clawler\\Windows";
            while (!Directory.Exists(DCPath) || !File.Exists(Path.Combine(DCPath, "DungeonClawler.exe")))
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Dungeon Clawler not found at {DCPath}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Press Enter to select your Dungeon Clawler folder or Escape to close that terminal");
                Console.ResetColor();
                var input = ConsoleKey.A;
                while (true)
                {
                    input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Enter)
                    {
                        DCPath = Utils.GetFolder();
                        break;
                    }
                    if (input == ConsoleKey.Escape)
                    {
                        Application.Exit();
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Dungeon Clawler found at {DCPath}");
            Console.ResetColor();

            if (Directory.Exists(Path.Combine(DCPath, "Bepinex")))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("BepInEx is already installed.");
                Console.ResetColor();
            }
            else
            {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"BepInEx is not installed at {DCPath}.\n do you want to install it? (Enter to accept and Escape to close this Installer)");
                Console.ResetColor();
                var input2 = ConsoleKey.A;
                while (input2 != ConsoleKey.Escape)
                {
                    input2 = Console.ReadKey(true).Key;
                    if (input2 == ConsoleKey.Enter)
                    {
                        BepinexInstall.InstallBepInEx();
                    }
                    if (input2 == ConsoleKey.Escape)
                    {
                        Application.Exit();
                    }
                }
                //ModBrowser.Init(Directory.Exists(Path.Combine(DCPath, "Bepinex")), DCPath);
                //bool continueBrowsing = true;
                //while (continueBrowsing) {continueBrowsing = ModBrowser.Update(); }
            }
            Console.WriteLine("Press any key to escape...");
            Console.ReadKey();
            
        }
        
    }
}