using Capstone.Shared.Models;
using MongoDB.Bson.Serialization.Attributes;


namespace Capstone.API.Models
{
	public class ContactMessage_DB
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }

		public User? User { get; set; }
		public string? Email { get; set; }
		public string Header { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string Topic { get; set; } = string.Empty;
		public bool Read { get; set; }
		public DateTime SentDate { get; set; }

		public ContactMessage GetSharedModel()
		{
			ContactMessage.MessageTopic translatedTopic = ContactMessage.MessageTopic.Other;
			if (Enum.TryParse<ContactMessage.MessageTopic>(Topic, out var asEnum))
			{
				translatedTopic = asEnum;
			}

			return new ContactMessage
			{
				Id = Id,
				User = User,
				Email = Email,
				Header = Header,
				Content = Content,
				Topic = translatedTopic,
				Read = Read,
				SentDate = SentDate
			};
		}
	}
}
