using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Capstone.API.Models
{
	public class User_DB
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfNull]
		public string? Id { get; set; }

		[BsonElement("username")]
		public string Username { get; set; }

		[BsonElement("password")]
		public string Password { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }

		[BsonElement("role")]
		public string Role { get; set; }
	}
}
