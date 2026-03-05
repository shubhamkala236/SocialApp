using System;
using System.Collections.Generic;
using System.Text;

namespace InteractionService.Domain.Entity
{
	public class SavedPost
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid PostId { get; set; }
		public Guid UserId { get; set; }

		// Snapshot of post data so we don't need to call PostService
		public string PostTitle { get; set; } = string.Empty;
		public string PostContent { get; set; } = string.Empty;
		public string PostUsername { get; set; } = string.Empty;
		public string? PostImageUrl { get; set; }
		public DateTime PostCreatedAt { get; set; }
		public DateTime SavedAt { get; set; } = DateTime.UtcNow;
	}

}
