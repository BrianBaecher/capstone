using Capstone.API.Models;
using Capstone.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Capstone.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ContactController : ControllerBase
	{
		private readonly IMongoCollection<ContactMessage_DB> _contactMessages;

		public ContactController(IMongoClient mongoClient)
		{
			var database = mongoClient.GetDatabase("Capstone");
			_contactMessages = database.GetCollection<ContactMessage_DB>("contactMessages");
		}

		[Authorize(Roles = "admin")]
		[HttpGet]
		public async Task<ActionResult<List<ContactMessage>>> Get([FromQuery] int? limit)
		{
			var query = _contactMessages.Find(_ => true);
			List<ContactMessage_DB> DBmessages;

			if (limit.HasValue)
			{
				DBmessages = await query.Limit(limit.Value).ToListAsync();
			}
			else
			{
				DBmessages = await query.ToListAsync();
			}

			List<ContactMessage> messages = new();
			foreach (var item in DBmessages)
			{
				messages.Add(item.GetSharedModel());
			}

			return Ok(messages);
		}

		[Authorize(Roles = "admin")]
		[HttpGet("{id:length(24)}")]
		public async Task<ActionResult<ContactMessage>> Get(string id)
		{
			var message = await _contactMessages.Find(m => m.Id == id).FirstOrDefaultAsync();
			if (message == null)
				return NotFound();
			return Ok(message.GetSharedModel());
		}

		[HttpPost]
		public async Task<ActionResult<ContactMessage>> Create([FromBody] ContactMessage message)
		{
			var dbMessage = new ContactMessage_DB()
			{
				User = message.User,
				Email = message.Email,
				Header = message.Header,
				Content = message.Content,
				Topic = message.Topic.ToString(),
				IsTestimonial = message.IsTestimonial,
				TestimonialDisplayName = message.TestimonialDisplayName,
				SentDate = message.SentDate,
			};

			await _contactMessages.InsertOneAsync(dbMessage);
			return CreatedAtAction(nameof(Get), new { id = dbMessage.Id }, dbMessage.GetSharedModel());
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var result = await _contactMessages.DeleteOneAsync(m => m.Id == id);
			if (result.DeletedCount == 0)
				return NotFound();
			return NoContent();
		}

		[Authorize(Roles = "admin")]
		[HttpGet("unread")]
		public async Task<IActionResult> GetUnreadCount([FromQuery] string topic)
		{
			var ct = await _contactMessages.CountDocumentsAsync((x) => x.Topic == topic && x.Read == false);
			return Ok(ct);
		}

		[Authorize(Roles = "admin")]
		[HttpPatch("{id:length(24)}/read")]
		public async Task<IActionResult> UpdateReadStatus(string id, [FromBody] bool isRead)
		{
			var update = Builders<ContactMessage_DB>.Update.Set(x => x.Read, isRead);
			var result = await _contactMessages.UpdateOneAsync(x => x.Id == id, update);

			if (result.ModifiedCount == 0) return NotFound();

			return NoContent();
		}
	}
}
