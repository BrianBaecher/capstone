using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UploadController : ControllerBase
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

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", dto.FileName);

			await System.IO.File.WriteAllBytesAsync(filePath, bytes);

			return Ok(new { FileName = dto.FileName });
		}
	}
}
