using System;
using HtmlAgilityPack;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace RimworldMMD_Linux
{
    class MainClass
    {
        private static HtmlWeb html = new HtmlWeb();
        private static String steamcmdLbl, pn;
        private static StringBuilder sb = new StringBuilder();
        private static HtmlDocument doc;
        public static void Main(string[] args)
        {
            Console.Title = "Rimworld Mod Scraper Linux";
            steamcmdLbl = "steamcmd";

            Q:
            Console.Write("This is the (Dev) linux version of Rimworld Mod Scraper (RMM).\nYou should have steamcmd installed. The command to install steamcmd for arch linux for example, would be 'yay -S steamcmd' but should not be run as root.\nPackage link: ");
            ConsoleColor pc = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("https://developer.valvesoftware.com/wiki/SteamCMD");
            Console.ForegroundColor = pc;
            Console.Write("\nWould you like to continue(1), exit(2) or attempt the aforementioned command(3)?: ");
            char c = Console.ReadKey().KeyChar;
            if (c=='3') {
                ProcessStartInfo zstartInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "yay -S steamcmd"  };
                Process zproc = new Process() { StartInfo = zstartInfo, };
                Console.Write("\n\n");
                Console.WriteLine("Log: \n");
                pc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                RunProcessAsync(zproc).Wait();
                Console.ForegroundColor = pc;
            }
            else if (c=='2')
            {
                pc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nExiting having not made any changes...");
                Console.ForegroundColor =pc;
                Environment.Exit(0);
            }
            else if (c!='1')
            {
                pc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nError, you did not respond with either the: '1' Key, '2' Key, or '3' Key.");
                Console.ForegroundColor = pc;
                goto Q;
            }
            pc= Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nContinuing...");
            Console.ForegroundColor = pc;
            sb.Append("+login anonymous ");
            Console.Write("Insert the number of mods would you like to download: ");
            Int32 num = Int32.Parse(Console.ReadLine());
            for (Int32 i = 0; i != num; ++i) LoadMod(i);
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = steamcmdLbl+" "+sb};
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
        }
        private static void LoadMod(Int32 index)
        {
            String name = @"https://steamcommunity.com/workshop/browse/?appid=294100&browsesort=trend&section=readytouseitems&days=-1&actualsort=trend&p=" + ((Int32)(index / 30) + 1).ToString();
            if (name != pn)
            {
                doc = html.Load(name);
                pn = name;
            }
            // This parses html
            // Essentialy we are just getting the ID of the mod
            // There are 30 mods on each page
            // If this breaks, you will just have to parse html to get the ID of the mods in a new way. I can easily do it if I realize it needs to be fixed.
            sb.Append("+workshop_download_item 294100 " + doc.DocumentNode.Descendants(0).Where(n => n.HasClass("workshopBrowseItems")).First().ChildNodes.Where(x => x.HasClass("workshopItem")).ElementAt(index % 30).ChildNodes.Where(x => x.Name == "a").First().OuterHtml.Substring(9).Split('&')[0].Split('=').Last().ToString() + ' ');
        }
        private static Task<Int32> RunProcessAsync(Process process)
        {
            TaskCompletionSource<Int32> tcs = new TaskCompletionSource<Int32>();

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }


    }
}
