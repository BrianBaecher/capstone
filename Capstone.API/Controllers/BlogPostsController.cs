using Capstone.API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BlogPostsController : ControllerBase
	{
		private readonly IMongoCollection<BlogPost_DB> _blogPosts;

		public BlogPostsController(IMongoClient mongoClient)
		{
			var database = mongoClient.GetDatabase("Capstone");
			_blogPosts = database.GetCollection<BlogPost_DB>("blogPosts");
		}

		[HttpGet]
		public async Task<ActionResult<List<BlogPost_DB>>> Get([FromQuery] int? limit)
		{
			var query = _blogPosts.Find(_ => true);

			List<BlogPost_DB> posts = new();

			if (limit.HasValue)
			{
				posts = await query.Limit(limit.Value).ToListAsync();
			}
			else
			{
				posts = await query.ToListAsync();
			}

			return Ok(posts);
		}


		[HttpGet("{id:length(24)}")]
		public async Task<ActionResult<BlogPost_DB>> Get(string id)
		{
			var post = await _blogPosts.Find(p => p.Id == id).FirstOrDefaultAsync();
			if (post == null)
				return NotFound();
			return Ok(post);
		}

		[HttpPost]
		public async Task<ActionResult<BlogPost_DB>> Create(BlogPost_DB post)
		{
			await _blogPosts.InsertOneAsync(post);
			return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
		}

		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, BlogPost_DB updatedPost)
		{
			var result = await _blogPosts.ReplaceOneAsync(p => p.Id == id, updatedPost);
			if (result.MatchedCount == 0)
				return NotFound();
			return NoContent();
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _blogPosts.DeleteOneAsync(p => p.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}
	}
}