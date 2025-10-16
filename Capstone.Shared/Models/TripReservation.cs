namespace Capstone.Shared.Models
{
	public class TripReservation
	{
		public string? Id { get; set; }

		public required Destination Destination { get; set; }

		public required User User { get; set; }

		public TimeSpan DurationOfStay { get; set; }

		public float Price { get; set; }
	}
}
