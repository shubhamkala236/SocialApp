using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
	// Published by AuthService when a new user registers
	public record UserRegisteredEvent
	{
		public Guid UserId { get; init; }
		public string Username { get; init; } = string.Empty;
		public string Email { get; init; } = string.Empty;
		public DateTime RegisteredAt { get; init; }
	}

	// Published by AuthService when a user deletes their account
	public record UserDeletedEvent
	{
		public Guid UserId { get; init; }
		public string Username { get; init; } = string.Empty;
		public DateTime DeletedAt { get; init; }
	}

	public record UserAvatarUpdatedEvent
	{
		public Guid UserId { get; init; }
		public string AvatarUrl { get; init; } = string.Empty;
	}
}
