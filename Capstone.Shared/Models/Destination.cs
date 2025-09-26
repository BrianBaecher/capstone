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

		public Destination Clone()
		{
			Destination other = new Destination();
			other.Id = Id;
			other.Code = Code;
			other.Name = Name;
			other.ImageFilename = ImageFilename;
			other.Description = Description;
			other.PricePerDay = PricePerDay;
			return other;
		}

		/// <summary>
		/// Compares two <see cref="Destination"/> objects and determines which fields have changed.
		/// </summary>
		/// <remarks>This method performs a field-by-field comparison of the two <see cref="Destination"/> objects and
		/// returns a <see cref="DestinationChanges"/> object that indicates whether each field has changed. The comparison
		/// assumes that the two objects represent the same entity (i.e., they share the same <see
		/// cref="Destination.Id"/>).</remarks>
		/// <param name="a">The first <see cref="Destination"/> object to compare. This is typically the original object.</param>
		/// <param name="b">The second <see cref="Destination"/> object to compare. This is typically the modified object.</param>
		/// <returns>A <see cref="DestinationChanges"/> object indicating which fields have changed between the two <see
		/// cref="Destination"/> objects.</returns>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="a"/> and <paramref name="b"/> objects have different <see cref="Destination.Id"/>
		/// values. This method is intended to compare copies or clones of the same <see cref="Destination"/> object, not
		/// distinct objects.</exception>
		public static DestinationChanges CompareFields(Destination a, Destination b)
		{
			if (a.Id != b.Id) throw new ArgumentException("CompareFields is only meant to be used on copies/clones of Destination objects. It is not meant to compare discrete Destination objects");

			return new DestinationChanges
			{
				CodeChanged = a.Code != b.Code,
				NameChanged = a.Name != b.Name,
				ImageFilenameChanged = a.ImageFilename != b.ImageFilename,
				DescriptionChanged = a.Description != b.Description,
				PricePerDayChanged = a.PricePerDay != b.PricePerDay,
			};
		}

		/// <summary>
		/// Represents an immutable record of changes to a <see cref="Destination"/> object, indicating which properties have been modified.
		/// </summary>
		public readonly struct DestinationChanges
		{
			public bool CodeChanged { get; init; }
			public bool NameChanged { get; init; }
			public bool ImageFilenameChanged { get; init; }
			public bool DescriptionChanged { get; init; }
			public bool PricePerDayChanged { get; init; }
		}
	}
}
