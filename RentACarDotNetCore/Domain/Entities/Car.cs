
namespace Domain.Entities
{

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;


    [BsonIgnoreExtraElements]
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("plate")]
        public string Plate { get; set; }

        [BsonElement("modelYear")]
        public int ModelYear { get; set; }

        [BsonElement("model")]
        public Model Model { get; set; }
    }
}
