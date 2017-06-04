using System.IO;
using System.Threading.Tasks;
using Luppa.Data;
using MongoDB.Driver;

namespace Luppa.Crawler
{
    public class ListService
    {
        public async Task StartCrawler()
        {
            var htmlList = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Crawler\allfiles.html");

            await new ListCrawler().ParseLinks(htmlList);
        }
    }
}