using Capstone.Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DestinationsController : ControllerBase
	{
		private readonly IMongoCollection<Destination> _destinations;

		public DestinationsController(IConfiguration configuration)
		{
			var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
			var database = client.GetDatabase("Capstone");
			_destinations = database.GetCollection<Destination>("destinations");
		}

		[HttpGet]
		public async Task<ActionResult<List<Destination>>> Get()
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

		[HttpPost]
		public async Task<ActionResult<Destination>> Create(Destination destination)
		{
			await _destinations.InsertOneAsync(destination);
			return CreatedAtRoute("GetDestination", new { id = destination.Id }, destination);
		}

		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, Destination updatedDestination)
		{
			var result = await _destinations.ReplaceOneAsync(d => d.Id == id, updatedDestination);
			if (result.MatchedCount == 0)
				return NotFound();
			return NoContent();
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _destinations.DeleteOneAsync(d => d.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}
	}
}