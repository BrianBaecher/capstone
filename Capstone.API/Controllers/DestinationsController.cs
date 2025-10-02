using Capstone.Api.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DestinationsController : ControllerBase
	{
		private readonly IMongoCollection<Destination_DB> _destinations;

		public DestinationsController(IConfiguration configuration)
		{
			var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
			var database = client.GetDatabase("Capstone");
			_destinations = database.GetCollection<Destination_DB>("destinations");
		}

		#region basic CRUD
		[HttpGet]
		public async Task<ActionResult<List<Destination_DB>>> Get()
		{
			var destinations = await _destinations.Find(_ => true).ToListAsync();
			return Ok(destinations);
		}

		[HttpGet("{id:length(24)}", Name = "GetDestination")]
		public async Task<ActionResult<Destination>> Get(string id)
		{
			var destination = await _destinations.Find(d => d.Id == id).FirstOrDefaultAsync();
			if (destination == null)
				return NotFound();
			return Ok(destination);
		}

		[Authorize]
		[HttpPost]
		public async Task<ActionResult<Destination_DB>> Create(Destination_DB destination)
		{
			await _destinations.InsertOneAsync(destination);
			return CreatedAtRoute("GetDestination", new { id = destination.Id }, destination);
		}

		[Authorize]
		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, Destination_DB updatedDestination)
		{
			var result = await _destinations.ReplaceOneAsync(d => d.Id == id, updatedDestination);
			if (result.MatchedCount == 0)
				return NotFound();
			return NoContent();
		}

		[Authorize]
		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _destinations.DeleteOneAsync(d => d.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}
		#endregion

		[HttpGet("destination-code/isunique")]
		public async Task<bool> GetIsDestinationCodeUnique([FromQuery] string code)
		{
			// note - destination code field has its uniqueness enforced by an index in the destinations collection. 
			// this method just checks to see if any destination currently has the 'code' string argument as its Code.
			var entry = await _destinations.Find(d => d.Code == code).FirstOrDefaultAsync();
			return entry == null;
		}

	}
}