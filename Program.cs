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
            Console.WriteLine("Starting crowler");
            Task.WaitAll(new BECCrawler().ParseLinks(new List<CrawlerLink>
            {
                new CrawlerLink { Url = "http://www.bec.sp.gov.br/BEC_Dispensa_UI/ui/BEC_DL_OC_ITEM.aspx?chave=&OC=B5QTJFaErQ5iX8OiiiFYjsHtgAcsNbhMWChWG%2bVEwwXOXXMFjHzAXwc50G0bTP%2fd&CA=" }
            }));
        }
    }
}
