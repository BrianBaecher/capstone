namespace Capstone.Shared.Models
{
	public class TravelPackage
	{
		public string? Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public List<string> Tags { get; set; } = new List<string>();

		public string Description { get; set; } = string.Empty;

		public string ImageResource { get; set; } = string.Empty;

		public Destination Destination { get; set; }
	}
}
