using MongoDB.Bson.Serialization.Attributes;

namespace Luppa.Data
{
    public class Product
    {
        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; }

        [BsonElement("productName")]
        public string ProductName { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("crawlerPrice")]
        public double CrawlerPrice { get; set; }

        [BsonElement("quantity")]
        public double Quantity { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }

        [BsonElement("totalPrice")]
        public double TotalPrice { get; set; }
        
        [BsonElement("totalCrawlerPrice")]
        public double TotalCrawlerPrice { get; set; }

        public override string ToString()
        {
            return ProductName;
        }
    }
}