using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace RentACarDotNetCore.Domain.Entities
{
    [CollectionName("JWTUsers")]
    [BsonIgnoreExtraElements]
    public class JWTUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long NationalIdentity { get; set; }
        public int DateOfBirthYear { get; set; }
    }
}
