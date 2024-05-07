using System;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Rimworld_Mass_Mod_Downloader {
	internal class Program {
        private static HtmlWeb html=new HtmlWeb();
		private static String steamcmdPath,pn;
		private static StringBuilder sb=new StringBuilder();
		private static HtmlDocument doc;
        public static void Main (String[] args) {
			Console.Title="Rimworld Mod Scraper";
			steamcmdPath="./steamcmd.exe";
			if (!File.Exists(@"/steamcmd.exe")) {
				Console.ForegroundColor=ConsoleColor.Red;
				Console.Write("FATAL ERROR\nClose and put steamcmd.exe (https://developer.valvesoftware.com/wiki/SteamCMD) in this folder and run it once or write the path to your steamcmd.exe file in a c# parsable string here: ");
				Console.ForegroundColor=ConsoleColor.White;
				steamcmdPath=Console.ReadLine();
            }
            sb.Append("+login anonymous ");
            Console.Write("Insert the number of mods would you like to download: ");
			Int32 num=Int32.Parse(Console.ReadLine());
			for (Int32 i=0;i!=num;++i) LoadMod(i);
			Process.Start(steamcmdPath,sb.ToString());
		}
		private static void LoadMod (Int32 index) {
			String name=@"https://steamcommunity.com/workshop/browse/?appid=294100&browsesort=trend&section=readytouseitems&days=-1&actualsort=trend&p="+((Int32)(index/30)+1).ToString();
            if (name!=pn) { 
				doc=html.Load(name);
				pn=name;
			}
            sb.Append("+workshop_download_item 294100 "+doc.DocumentNode.Descendants(0).Where(n=>n.HasClass("workshopBrowseItems")).First().ChildNodes.Where(x=>x.HasClass("workshopItem")).ElementAt(index%30).ChildNodes.Where(x=>x.Name=="a").First().OuterHtml.Substring(9).Split("&")[0].Split("=").Last().ToString()+' ');
		}

		
	}
}