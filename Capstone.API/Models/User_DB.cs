using Capstone.Shared.Models;
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


		public User GetSharedModel()
		{
			if (this.Id == null)
			{
				throw new InvalidOperationException("");
			}
			return new User()
			{
				Id = Id,
				Username = Username,
				Password = Password,
				Email = Email,
				Role = Role
			};
		}
	}
}
