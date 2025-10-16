namespace Capstone.Shared.Models
{
	public class ContactMessage
	{
		public string? Id { get; set; }
		public User? User { get; set; }
		public string? Email { get; set; }
		public string Header { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;

		public bool IsTestimonial { get; set; }

		public string? TestimonialDisplayName { get; set; }

		public MessageTopic Topic { get; set; }

		public bool Read { get; set; }

		public DateTime SentDate { get; set; }

		public enum MessageTopic
		{
			Support,
			Feedback,
			Sales,
			Other
		}
	}
}
