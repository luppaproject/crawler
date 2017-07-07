using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Luppa.Data
{
    public class MongoCollections : IMongoCollections
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
        public IMongoCollection<CrawlerLink> CrawlerLink => database.GetCollection<CrawlerLink>("crawlerLinks");
    }
}