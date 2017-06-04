using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Luppa.Data
{
    public class Bidding : BaseModel
    {
        [BsonElement("orderNumber")]
        public string OrderNumber { get; set; }

        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; }

        [BsonElement("productAlias")]
        public string ProductAlias { get; set; }

        [BsonElement("totalPrice")]
        public double TotalPrice { get; set; }

        [BsonElement("crawlerPrice")]
        public double CrawlerPrice { get; set; }

        [BsonElement("quantity")]
        public double Quantity { get; set; }

        [BsonElement("orderType")]
        public string OrderType { get; set; }

        [BsonElement("biddingUrl")]
        public string BiddingUrl { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }

        [BsonElement("products")]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}