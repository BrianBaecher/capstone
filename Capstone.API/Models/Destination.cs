using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.Api.Models
{
	public partial class Destination
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BsonIgnoreIfNull]
		public string? Id { get; set; }

		[BsonElement("code")]
		public string Code { get; set; }

		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("imageFilename")]
		public string ImageFilename { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }

		[BsonElement("pricePerDay")]
		public double PricePerDay { get; set; }
	}
}
