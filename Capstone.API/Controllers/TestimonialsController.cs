using Capstone.API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestimonialsController : ControllerBase
	{
		private readonly IMongoCollection<Testimonial> _testimonials;

		public TestimonialsController(IMongoClient mongoClient)
		{
			var database = mongoClient.GetDatabase("Capstone");
			_testimonials = database.GetCollection<Testimonial>("testimonials");
		}

		[HttpGet]
		public async Task<ActionResult<List<Testimonial>>> Get()
		{
			var testimonials = await _testimonials.Find(_ => true).ToListAsync();
			return Ok(testimonials);
		}

		[HttpGet("{id:length(24)}")]
		public async Task<ActionResult<Testimonial>> Get(string id)
		{
			var testimonial = await _testimonials.Find(t => t.Id == id).FirstOrDefaultAsync();
			if (testimonial == null)
				return NotFound();
			return Ok(testimonial);
		}

		[HttpPost]
		public async Task<ActionResult<Testimonial>> Create(Testimonial testimonial)
		{
			await _testimonials.InsertOneAsync(testimonial);
			return CreatedAtAction(nameof(Get), new { id = testimonial.Id }, testimonial);
		}

		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, Testimonial updatedTestimonial)
		{
			var result = await _testimonials.ReplaceOneAsync(t => t.Id == id, updatedTestimonial);
			if (result.MatchedCount == 0)
				return NotFound();
			return NoContent();
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _testimonials.DeleteOneAsync(t => t.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}
	}
}