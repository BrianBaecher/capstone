using Capstone.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.Api.Models
{
	public class Destination_DB
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



		public Destination GetSharedModel()
		{
			if (this.Id == null)
			{
				throw new InvalidOperationException("");
			}
			return new Destination()
			{
				Id = Id,
				Code = Code,
				Name = Name,
				ImageFilename = ImageFilename,
				Description = Description,
				PricePerDay = PricePerDay
			};
		}
	}
}
