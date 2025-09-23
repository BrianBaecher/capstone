using Capstone.Api.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TravelPackagesController : ControllerBase
	{
		private readonly IMongoCollection<TravelPackage_DB> _packages;
		private readonly IMongoCollection<Destination_DB> _destinations;

		public TravelPackagesController(IConfiguration configuration)
		{
			var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
			var database = client.GetDatabase("Capstone");
			_packages = database.GetCollection<TravelPackage_DB>("travelPackages");
			_destinations = database.GetCollection<Destination_DB>("destinations");
		}

		[HttpGet]
		public async Task<ActionResult<List<TravelPackage>>> Get()
		{
			var packages = await _packages.Find(_ => true).ToListAsync();
			var result = new List<TravelPackage>();

			foreach (var pkg in packages)
			{
				// TravelPackage_DB records do not store full Destination_DB objects, only their BsonId. So get the full obj from the _destinations collection, include it in response.
				var destinationDB = await _destinations.Find(d => d.Id == pkg.DestinationBsonId).FirstOrDefaultAsync();
				result.Add(new TravelPackage
				{
					Id = pkg.Id,
					Name = pkg.Name,
					Tags = pkg.Tags,
					Description = pkg.Description,
					ImageResource = pkg.ImageResource,
					Destination = destinationDB.GetSharedModel()
				});
			}

			return Ok(result);
		}

		[HttpGet("{id:length(24)}", Name = "GetTravelPackage")]
		public async Task<ActionResult<TravelPackage>> Get(string id)
		{
			var pkg = await _packages.Find(p => p.Id == id).FirstOrDefaultAsync();
			if (pkg == null)
				return NotFound();

			var destination = await _destinations.Find(d => d.Id == pkg.DestinationBsonId).FirstOrDefaultAsync();

			var ret = new TravelPackage
			{
				Id = pkg.Id,
				Name = pkg.Name,
				Tags = pkg.Tags,
				Description = pkg.Description,
				ImageResource = pkg.ImageResource,
				Destination = destination.GetSharedModel()
			};

			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<TravelPackage>> Create(TravelPackage_DB package)
		{
			await _packages.InsertOneAsync(package);
			return CreatedAtRoute("GetTravelPackage", new { id = package.Id }, package);
		}

		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, TravelPackage_DB updatedPackage)
		{
			var result = await _packages.ReplaceOneAsync(p => p.Id == id, updatedPackage);
			if (result.MatchedCount == 0)
				return NotFound();
			return NoContent();
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _packages.DeleteOneAsync(p => p.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}
	}
}