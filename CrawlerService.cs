using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Crawler;
using Luppa.Data;
using MongoDB.Driver;

namespace Luppa
{
    public class CrawlerService
    {
        public async Task StartCrawler()
        {
            var mongoCollection = new MongoCollections();

            var urls = await mongoCollection
                .CrawlerLink
                .Find(t => !t.IsBecParsed)
                .ToListAsync();

            await new BECCrawler().ParseLinks(urls);
        }
    }
}