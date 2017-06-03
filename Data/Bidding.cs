using MongoDB.Bson.Serialization.Attributes;

namespace Luppa.Data
{
    public class Bidding : BaseModel
    {
        [BsonElement("orderNumber")]
        public string OrderNumber { get; set; }

        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; }

        [BsonElement("productName")]
        public string ProductName { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("crawlerPrice")]
        public decimal CrawlerPrice { get; set; }

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("orderType")]
        public string OrderType { get; set; }

        [BsonElement("score")]
        public string Score { get; set; }
    }
}