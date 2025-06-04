using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonClawlerBepinexInstaller
{
    internal class ModBrowser
    {
        static List<string> links = new List<string>();
        static bool hasBepinex = false;
        static string path;
        static int selectedIndex = 0;
        public static void Init(bool BepinexInstalled, string gamePath)
        {
            ReloadModsList();
            hasBepinex = BepinexInstalled;
            path = gamePath;
        }

        static int scrollOffset = 0;

        public static bool Update()
        {
            int maxVisible = Console.WindowHeight - 1;
            int displayCount = links.Count;


            if (selectedIndex < scrollOffset)
                scrollOffset = selectedIndex;
            else if (selectedIndex >= scrollOffset + maxVisible)
                scrollOffset = selectedIndex - maxVisible + 1;

            Console.Clear();

            Console.SetCursorPosition(0, 0);


            for (int i = scrollOffset; i < Math.Min(displayCount, scrollOffset + maxVisible); i++)
            {
                bool downpatch = false;
                string link = links[i];
                if (link.StartsWith("*"))
                {
                    link = link.Substring(1);
                    downpatch = true;
                }

                link = link.Split('/').Last().Replace('-', ' ');
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"> {link}");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"  {link}");
                }
                if (downpatch)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" (Downpatch)");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }


            var input = Console.ReadKey(true).Key;

            if (input == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + displayCount) % displayCount;
            }
            else if (input == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % displayCount;
            }
            else if (input == ConsoleKey.R)
            {
                ReloadModsList();
                Console.WriteLine("Mod list reloaded.");
            }
            else if (input == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.Write("Exiting Mod Browser");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }

                return false;
            }
            else if (input == ConsoleKey.Enter)
            {
                Console.WriteLine(links[selectedIndex]);
                string[] selectedLink = links[selectedIndex].Replace("*","").Replace("https://github.com/", "").Split('/');
                ShowModDetails(selectedLink[0], selectedLink[1]);
            }
            return true;
        }

        public static void ReloadModsList()
        {
            using (var client = new WebClient())
            using (Stream stream = client.OpenRead("https://raw.githubusercontent.com/Bestcreeper-code/Dungeon-Clawler-ModLibrary/main/ModsList.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                links = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            }
        }

        public static void ShowModDetails(string author, string modName)
        {
            Console.WriteLine($"Fetching details for {modName} by {author}...");
            Thread.Sleep(3000); // Simulate loading time
            List<string> lines;
            string desc = "", name = "", authorName = "", version = "",link ="",raw = "";
            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "MyApp");
                using (Stream stream = client.OpenRead($"https://api.github.com/repos/{author}/{modName}"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    content = content.Replace(",", "\n");
                    lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("\"description\"") && desc == "")
                        {
                            desc = line.Split(':')[1].Trim().Trim('"');
                        }
                        else if (line.StartsWith("\"name\"") && name == "")
                        {
                            name = line.Split(':')[1].Trim().Trim('"');
                        }
                        else if (line.StartsWith("\"owner\"") && authorName == "")
                        {
                            authorName = line.Split(':').Last().Trim().Trim('"');
                        }
                    }
                }
                client.Headers.Add("User-Agent", "MyApp");
                using (Stream stream = client.OpenRead($"https://api.github.com/repos/{author}/{modName}/releases/latest"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    content = content.Replace(",", "\n");
                    lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("\"tag_name\"") && version == "")
                        {
                            version = line.Split(':')[1].Trim().Trim('"');
                            
                        }
                        if (line.StartsWith("\"browser_download_url\"") && link == "")
                        {

                            link = line.Replace("\"browser_download_url\":", "").Trim().Trim(']', '}').Trim('"');
                            Console.WriteLine(line);
                            Console.WriteLine("url:  " + link);
                            Thread.Sleep(10000);
                            
                        }
                        if (line.StartsWith("\"zipball_url\"") && raw == "")
                        {
                            raw = line.Replace("\"zipball_url\":", "").Trim().Trim('"');
                        }
                        
                    }

                }
            }
            Console.Clear();
            Console.WriteLine($"Mod Name: {name}");
            Console.WriteLine($"Author: {authorName}");
            Console.WriteLine($"Version: {version}");
            Console.WriteLine($"Description: {desc}");
            Console.WriteLine("\nPress Enter to download or Escape to return to the mod list.");
            var input = ConsoleKey.A;
            byte selected = 0;
            while (input != ConsoleKey.Escape) { 
                input = Console.ReadKey(true).Key;
                if (input == ConsoleKey.Enter && selected ==0)
                {
                    Console.WriteLine("confirm download?");
                    selected = 1;
                }
                if (selected == 1 && input == ConsoleKey.Enter)
                {
                    Console.WriteLine(link == ""+" link");
                    Console.WriteLine(link + "    " + raw);
                    Thread.Sleep(2222);
                    DownloadMod(string.IsNullOrEmpty(link)? link : raw, path);
                    break;
                }

            }
        }
        public static void DownloadMod(string link,string destination)
        {
            Console.WriteLine(link + "  -->  "+ destination);
            Thread.Sleep(1000);
            Utils.Download(link, destination);
        }
    }
}
