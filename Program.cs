using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Crawler;
using Luppa.Data;

namespace Luppa
{
    class Program
    {
        static void Main(string[] args)
        {
            //var crowler = args.Length > 0 ? args[0] : "bec";
            var crowler = "buscape";
            // var crowler = "lista";

            Console.WriteLine("Starting crowler");

            switch (crowler)
            {
                // case "lista":
                //     Task.WaitAll(new ListService().StartCrawler());
                //     break;
                case "buscape":
                    Task.WaitAll(new BuscapeService().StartCrawler());
                    break;
                default:
                    Task.WaitAll(new CrawlerService().StartCrawler());
                    break;
            }
        }
    }
}
