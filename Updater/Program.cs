using System;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Updater
{
    public class Program
    {
        private static string url = @"https://raw.githubusercontent.com/thebetioplane/OsuReplayEditorV2/master/Download/";

        public static void Main(string[] args)
        {
            try
            {
                string localVersion = "not found";
                string webVersion = "222";
                if (File.Exists("files/version.txt"))
                {
                    localVersion = File.ReadAllText("files/version.txt");
                }
                else
                {
                    Directory.CreateDirectory("files");
                    File.WriteAllText("files/version.txt", localVersion);
                }
                Console.WriteLine("Replay Editor local version {0}", localVersion);
                using (StreamReader reader = new StreamReader((HttpWebRequest.CreateHttp(url + "files/Version.txt").GetResponse() as HttpWebResponse).GetResponseStream()))
                {
                    webVersion = reader.ReadToEnd();
                }
                if (!localVersion.Equals(webVersion))
                {
                    Console.WriteLine("Downloading update...");
                    using (WebClient downloader = new WebClient())
                    {
                        downloader.DownloadFile(url + "files/editor.exe", "files/editor.exe.dl");
                    }
                    File.Delete("files/editor.exe");
                    File.Move("files/editor.exe.dl", "files/editor.exe");
                    File.WriteAllText("files/version.txt", webVersion);
                }
                Process.Start(Directory.GetCurrentDirectory() + "/files/editor.exe");
            }
            catch
            {
                Console.WriteLine("An error has occured during updating.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
