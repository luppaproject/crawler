using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Luppa
{
    public class BaseModel
    {
        public ObjectId Id { get; set; }
        
        [BsonElement("registerAt")]
        public DateTime RegisterAt { get; set; }

        public BaseModel()
        {
            Id = new ObjectId();
            RegisterAt = DateTime.Now;
        }
    }
}