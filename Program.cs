using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Crawler;
using Luppa.Data;
using Luppa.Services.BECList;

namespace Luppa
{
    class Program
    {
        // lista = get a new list of bidding inside BEC website
        // buscape = execute a Buscape's API using open links inside the database
        // bec/no args = execute a BEC's website scrapper and crawler
        static void Main(string[] args)
        {
            var crawler = args.Length > 0 ? args[0] : "bec";

            Console.WriteLine("Starting luppa service... WE ARE AGAINST CORRUPTION!");

            switch (crawler)
            {
                case "lista":
                    System.Console.WriteLine("Starting BEC's bidding scraper...");
                    Task.WaitAll(new BECListService().StartService());
                    break;
                case "buscape":
                    System.Console.WriteLine("Starting buscape crawler...");
                    Task.WaitAll(new BuscapeService().StartCrawler());
                    break;
                default:
                    System.Console.WriteLine("Starting bec scraper...");
                    Task.WaitAll(new CrawlerService().StartCrawler());
                    break;
            }
        }
    }
}
