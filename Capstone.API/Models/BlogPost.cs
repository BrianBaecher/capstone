using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.API.Models
{
	public class BlogPost
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfNull]
		public string? Id { get; set; }

		[BsonElement("title")]
		public string Title { get; set; }

		[BsonElement("date")]
		public DateTime DateTime { get; set; }

		[BsonElement("content")]
		public string Content { get; set; }

	}
}
