using Capstone.Shared.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Capstone.API.Models
{
	public class TripReservation_DB
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		public required User User { get; set; }

		public required Destination Destination { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public float Price { get; set; }
	}
}
