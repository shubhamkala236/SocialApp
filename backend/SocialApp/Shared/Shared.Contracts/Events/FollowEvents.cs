using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
	public record UserFollowedEvent
	{
		public Guid FollowerId { get; init; }
		public string FollowerUsername { get; init; } = string.Empty;
		public Guid FollowingId { get; init; }
		public string FollowingUsername { get; init; } = string.Empty;
		public DateTime FollowedAt { get; init; }
	}

	// Published by UserService when someone unfollows a user
	public record UserUnfollowedEvent
	{
		public Guid FollowerId { get; init; }
		public Guid FollowingId { get; init; }
		public DateTime UnfollowedAt { get; init; }
	}
}
