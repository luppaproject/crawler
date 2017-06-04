using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Luppa.Data
{
    public class MongoCollections
    {
        private readonly IMongoDatabase database;

        public MongoCollections()
        {
            this.database = MongoContext.GetDatabase();
        }

        public MongoCollections(IMongoDatabase database)
        {
            this.database = database;
        }

        public IMongoCollection<Bidding> Bidding => database.GetCollection<Bidding>("biddings");
        public IMongoCollection<CrawlerLink> CrowlerLink => database.GetCollection<CrawlerLink>("crawlerLinks");
    }
}