using System;
using System.Collections.Generic;
using System.Text;

namespace PostService.Domain
{
	public class Post
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public string? ImageUrl { get; set; }        // Cloudinary URL
		public string? ImagePublicId { get; set; }   // Cloudinary PublicId (needed for deletion)
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}
}
