using MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Luppa.Data
{
    public class CrawlerLink : BaseModel
    {
        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("isBecParsed")]
        public bool IsBecParsed { get; set; }

        [BsonElement("isZoomParsed")]
        public bool IsZoomParsed { get; set; }

        [BsonElement("isBuscapeParsed")]
        public bool IsBuscapeParsed { get; set; }
    }
}