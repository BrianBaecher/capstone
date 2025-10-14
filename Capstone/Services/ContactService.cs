using Capstone.Shared.Models;
using System.Net.Http.Json;

namespace Capstone.Services
{
	public class ContactService
	{
		private readonly HttpClient _httpClient;
		const string URI = "api/contact";

		public ContactService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<ContactMessage>> GetMessagesAsync(int? limit = null)
		{
			string url = URI;
			if (limit.HasValue)
				url += $"?limit={limit.Value}";

			return await _httpClient.GetFromJsonAsync<List<ContactMessage>>(url) ?? new();
		}

		public async Task<ContactMessage?> GetMessageAsync(string id)
		{
			return await _httpClient.GetFromJsonAsync<ContactMessage>($"api/contact/{id}");
		}

		public async Task<ContactMessage?> CreateMessageAsync(ContactMessage message)
		{
			var response = await _httpClient.PostAsJsonAsync("api/contact", message);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<ContactMessage>();
		}

		public async Task<bool> DeleteMessageAsync(string id)
		{
			var response = await _httpClient.DeleteAsync($"api/contact/{id}");
			return response.IsSuccessStatusCode;
		}

		public async Task<int?> GetUnreadCountAsync(string topic)
		{
			var res = await _httpClient.GetAsync($"{URI}/unread?topic={topic}");

			if (res.IsSuccessStatusCode)
			{
				var s = await res.Content.ReadAsStringAsync();

				if (int.TryParse(s, out int count))
				{
					return count;
				}
			}
			return null;
		}

		public async Task<bool> UpdateMessageReadStatusAsync(string msgID, bool isRead)
		{
			var jsonContent = JsonContent.Create(isRead);
			var res = await _httpClient.PatchAsync($"{URI}/{msgID}/read", jsonContent);

			return res.IsSuccessStatusCode;
		}
	}
}
