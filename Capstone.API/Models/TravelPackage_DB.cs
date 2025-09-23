using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.Api.Models
{
	public class TravelPackage_DB
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfNull]
		public string? Id { get; set; }

		[BsonElement("name")]
		public string Name { get; set; } = string.Empty;

		[BsonElement("tags")]
		public List<string> Tags { get; set; } = new List<string>();

		[BsonElement("description")]
		public string Description { get; set; } = string.Empty;

		[BsonElement("imageResource")]
		public string ImageResource { get; set; } = string.Empty;

		[BsonElement("destinationId")]
		public string DestinationBsonId { get; set; } = string.Empty;
	}
}