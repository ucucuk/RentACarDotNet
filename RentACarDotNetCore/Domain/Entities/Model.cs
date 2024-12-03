
namespace Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Model
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("brand")]
        public Brand Brand { get; set; }

        [BsonElement("cars")]
        public List<Car> Cars { get; set; }
    }


}
