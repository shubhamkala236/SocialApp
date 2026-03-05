using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
	public record PostLikedEvent
	{
		public Guid PostId { get; init; }
		public Guid UserId { get; init; }
		public string Username { get; init; } = string.Empty;
		public Guid PostOwnerId { get; init; }
		public DateTime LikedAt { get; init; }
	}
}
