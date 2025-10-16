using Capstone.Shared.Models;
using System.Net.Http.Json;

namespace Capstone.Services
{
	public class ReservationService
	{
		private readonly HttpClient _httpClient;
		const string URI = "api/reservations";


		public ReservationService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> CreateReservationAsync(TripReservation reservation)
		{
			var res = await _httpClient.PostAsJsonAsync<TripReservation>(URI, reservation);

			return res.IsSuccessStatusCode;
		}

		public async Task<List<TripReservation>> GetUserReservations(User user)
		{
			string endpoint = $"{URI}?userId={user.Id}";
			return await _httpClient.GetFromJsonAsync<List<TripReservation>>(endpoint) ?? new();
		}

		public async Task<bool> DeleteReservationAsync(TripReservation reservation)
		{
			string endpoint = $"{URI}?id={reservation.Id}";

			var res = await _httpClient.DeleteAsync(endpoint);

			return res.IsSuccessStatusCode;
		}
	}
}
