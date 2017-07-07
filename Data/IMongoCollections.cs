using MongoDB.Driver;

namespace Luppa.Data
{
    public interface IMongoCollections
    {
        IMongoCollection<Bidding> Bidding { get; }
        IMongoCollection<CrawlerLink> CrawlerLink { get; }
    }
}