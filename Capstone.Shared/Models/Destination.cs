namespace Capstone.Shared.Models
{
	public class Destination
	{
		public string? Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string ImageFilename { get; set; }
		public string Description { get; set; }
		public double PricePerDay { get; set; }

		public string GetImageUrl(string baseApiUrl)
		{
			string imgUrl = $"{baseApiUrl}/images/destinations/{ImageFilename}";
			//Console.WriteLine(imgUrl);
			return imgUrl;
		}
	}
}
