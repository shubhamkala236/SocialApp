using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
	public record PostDeletedEvent
	{
		public Guid PostId { get; init; }
		public Guid UserId { get; init; }
		public DateTime DeletedAt { get; init; }
	}

	// Published by PostService when a post is created
	public record PostCreatedEvent
	{
		public Guid PostId { get; init; }
		public Guid UserId { get; init; }
		public string Username { get; init; } = string.Empty;
		public string Title { get; init; } = string.Empty;
		public DateTime CreatedAt { get; init; }
	}
}
