using ImageMagick;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ImageController : ControllerBase
	{
		public class ImageUploadDto
		{
			public string Base64Image { get; set; }
			public string FileName { get; set; }
		}

		[HttpPost("destination-image")]
		public async Task<IActionResult> UploadDestinationImage([FromBody] ImageUploadDto dto)
		{
			if (string.IsNullOrEmpty(dto.Base64Image) || string.IsNullOrEmpty(dto.FileName))
				return BadRequest();

			// Remove the data URL prefix
			var base64Data = dto.Base64Image.Substring(dto.Base64Image.IndexOf(",") + 1);
			var bytes = Convert.FromBase64String(base64Data);

			// using imageMagick to take base data and create avif file
			using var image = new MagickImage(bytes);

			var avifFileName = Path.ChangeExtension(dto.FileName, ".avif");
			var avifFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "destinations", avifFileName);

			image.Format = MagickFormat.Avif;

			await image.WriteAsync(avifFilePath);

			return Ok();
		}

		[HttpGet("destination-image/namecheck")]
		public IActionResult CheckFilenameAvailability([FromQuery] string name)
		{
			if (string.IsNullOrEmpty(name)) return BadRequest("You must enter a filename for uploaded images");

			var destinationsImageFileDirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "destinations");

			string[] existingFilenames = Directory.GetFiles(destinationsImageFileDirPath);

			foreach (var f in existingFilenames)
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileNameWithoutExtension(f), name) == 0)
				{
					return Conflict($"Cannot use \"{name}\" as a destination image filename, as a similarly named file already exists.");
				}
			}

			return Ok();
		}
	}
}
