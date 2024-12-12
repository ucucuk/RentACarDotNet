using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace RentACarDotNetCore.Domain.Entities
{
    [CollectionName("users")]
    [BsonIgnoreExtraElements]
    public class User : MongoIdentityUser
    {
        public User()
        {
            CreatedDate = DateTime.Now;
            Status = 1;
        }
        public DateTime CreatedDate { get; set; }

        public short Status { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalIdentity { get; set; }
        public int DateOfBirthYear { get; set; }
    }
}
