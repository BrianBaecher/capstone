using Capstone.API.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ReservationsController : ControllerBase
	{
		private readonly IMongoCollection<TripReservation_DB> _reservations;
		private readonly IMongoCollection<User_DB> _users;

		public ReservationsController(IMongoClient mongoClient)
		{
			var db = mongoClient.GetDatabase("Capstone");
			_reservations = db.GetCollection<TripReservation_DB>("reservations");
			_users = db.GetCollection<User_DB>("users");
		}


		[HttpGet]
		public async Task<IActionResult> GetUserReservations([FromQuery] string userId)
		{
			// check user exist
			var userFind = await _users.Find((x) => x.Id == userId).FirstOrDefaultAsync();

			if (userFind == null) return BadRequest();

			var reservations = await _reservations.Find((x) => x.User.Id == userId).ToListAsync();

			return Ok(reservations);
		}

		[HttpGet("{id:length(24)}")]
		public async Task<IActionResult> GetReservationById(string id)
		{
			var res = await _reservations.Find((x) => x.Id == id).FirstOrDefaultAsync();
			if (res == null) return NotFound();
			return Ok(res);
		}

		[HttpPost]
		public async Task<IActionResult> CreateReservationAsync([FromBody] TripReservation reservation)
		{
			if (reservation == null) return BadRequest();

			TripReservation_DB dbModel = new()
			{
				Destination = reservation.Destination,
				User = reservation.User,
				DurationOfStay = reservation.DurationOfStay,
				Price = reservation.Price,
			};

			await _reservations.InsertOneAsync(dbModel);

			if (dbModel.Id != null)
			{
				// Id assigned by mongo driver, indication of successful insert
				return Ok();
			}

			return StatusCode(500);
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> DeleteReservationAsync(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest();

			var res = await _reservations.DeleteOneAsync((x) => x.Id == id);

			if (res.DeletedCount == 0) return NotFound();

			return Ok();
		}

	}
}
