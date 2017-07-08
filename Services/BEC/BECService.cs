using System;
using System.Threading.Tasks;
using Luppa.Data;
using Luppa.Services.Common;
using MongoDB.Driver;

namespace Luppa.Services.BEC
{
    public class BECService : ILuppaService
    {
        public async Task StartService()
        {
            var mongoCollection = new MongoCollections();

            var urls = await mongoCollection
                .CrawlerLink
                .Find(t => !t.IsBecParsed)
                .ToListAsync();

            var becScraper = new BECScraper(mongoCollection);

            await becScraper.StartScraper(urls);
        }
    }
}