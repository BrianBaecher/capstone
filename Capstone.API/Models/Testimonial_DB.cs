using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.API.Models
{
	public class Testimonial_DB
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfNull]
		public string? Id { get; set; }

		[BsonElement("author")]
		public string Author { get; set; }

		[BsonElement("content")]
		public string Content { get; set; }
	}
}
