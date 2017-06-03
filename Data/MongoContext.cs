using System.IO;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Luppa.Data
{
    public static class MongoContext
    {
        public static IMongoDatabase GetDatabase()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl("mongodb://localhost:27017")
            );

            var client = new MongoClient(settings);
            var database = client.GetDatabase("luppa");

            return database;
        }
    }
}