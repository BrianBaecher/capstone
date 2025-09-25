using System.Net.Http.Json;

public class ImageUploadDto
{
	public string Base64Image { get; set; }
	public string FileName { get; set; }
}

public class ImageService
{
	private readonly HttpClient _httpClient;
	private const string URI = "api/image/destination-image";

	public ImageService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<bool> UploadDestinationImageAsync(string base64Image, string fileName)
	{
		var dto = new ImageUploadDto
		{
			Base64Image = base64Image,
			FileName = fileName
		};

		var response = await _httpClient.PostAsJsonAsync(URI, dto);
		return response.IsSuccessStatusCode;
	}

	public async Task<HttpResponseMessage> CheckDestinationImageFilenameAvailableAsync(string name)
	{
		string endpoint = URI + $"/namecheck?name={name}";

		return await _httpClient.GetAsync(endpoint);
	}
}