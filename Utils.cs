using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace DungeonClawlerBepinexInstaller
{
    internal class Utils
    {

        public static string GetFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    return folderBrowserDialog.SelectedPath;
                }
            }
            return "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dungeon Clawler";
        }
        
        public static string Download(string url, string destinationPath)
        {
            string fileName = url.Split('/').Last();
            string filePath = Path.Combine(destinationPath, fileName);
            Console.WriteLine($"Downloading {url} to {destinationPath}");
            if (File.Exists(filePath))
            {
                return "X";
            }

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    Console.CursorLeft = 0;
                    Console.Write($"\rProgress: {e.ProgressPercentage}%");
                };
                client.DownloadFileCompleted += (s, e) =>
                {
                    Console.WriteLine("\nDownload complete.");
                };
                client.DownloadFileAsync(new Uri(url), filePath);
                while (client.IsBusy)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            return filePath;
        }
        public static void ExtractZip(string zipFilePath, string destinationPath)
        {
            string filename = Path.GetFileName(zipFilePath);
            Console.WriteLine($"Extracting {filename} to {destinationPath}");
            
            ZipFile.ExtractToDirectory(zipFilePath, destinationPath);
            File.Delete(zipFilePath);
            Console.WriteLine($"{filename} extracted to {destinationPath}");
        }
    }
}
