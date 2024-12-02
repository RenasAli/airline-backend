using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models.MongoDB
{
    public class AirplaneMongo
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement("name")]
        public string Name  { get; set; }

        [BsonElement("airlineId")]
        public int AirplanesAirlineId { get; set; }

        [BsonElement("economyClassSeats")]
        public int EconomyClassSeats { get; set; }

        [BsonElement("businessClassSeats")]
        public int BusinessClassSeats { get; set; }

        [BsonElement("firstClassSeats")]
        public int FirstClassSeats { get; set; }
    }
}
