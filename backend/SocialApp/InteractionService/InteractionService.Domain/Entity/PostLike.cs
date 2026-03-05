using System;
using System.Collections.Generic;
using System.Text;

namespace InteractionService.Domain.Entity
{
	public class PostLike
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid PostId { get; set; }
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
