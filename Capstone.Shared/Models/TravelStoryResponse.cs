namespace Capstone.Shared.Models
{
	public class TravelStoryResponse
	{
		public string Status { get; set; }
		public string Copyright { get; set; }
		public string Section { get; set; }
		public string Last_Updated { get; set; }
		public int Num_Results { get; set; }
		public List<TravelStory> Results { get; set; }
	}

	public class TravelStory
	{
		public string Section { get; set; }
		public string Subsection { get; set; }
		public string Title { get; set; }
		public string Abstract { get; set; }
		public string Url { get; set; }
		public string Uri { get; set; }
		public string Byline { get; set; }
		public string Item_Type { get; set; }
		public string Updated_Date { get; set; }
		public string Created_Date { get; set; }
		public string Published_Date { get; set; }
		public List<string> Des_Facet { get; set; }
		public List<string> Org_Facet { get; set; }
		public List<string> Per_Facet { get; set; }
		public List<string> Geo_Facet { get; set; }
		public List<Multimedia> Multimedia { get; set; }
		public string Short_Url { get; set; }
	}

	public class Multimedia
	{
		public string Url { get; set; }
		public string Format { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public string Type { get; set; }
		public string Subtype { get; set; }
		public string Caption { get; set; }
		public string Copyright { get; set; }
	}
}
