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

            var urls = await mongoCollection
                .CrowlerLink
                .Find(t => t.IsBecParsed && !t.IsBuscapeParsed)
                .ToListAsync();

            await new BuscapeCrawler().ParseLinks(urls);
        }
    }
}