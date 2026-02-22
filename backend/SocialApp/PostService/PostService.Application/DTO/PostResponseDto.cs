using System;
using System.Collections.Generic;
using System.Text;

namespace PostService.Application.DTO
{
	public class PostResponseDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public string? ImageUrl { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
