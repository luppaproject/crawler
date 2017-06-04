using System.Threading.Tasks;
using Luppa.Data;
using MongoDB.Driver;

namespace Luppa.Crawler
{
    public class BuscapeService
    {
        public async Task StartCrawler()
        {
            var mongoCollection = new MongoCollections();

            var biddings = await mongoCollection
                .Bidding
                .Find(t => t.CrawlerPrice == 0)
                .ToListAsync();

            await new BuscapeCrawler().ParseLinks(biddings);
        }
    }
}