using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTO
{
	public class UserProfileDto
	{
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? Bio { get; set; }
		public string? AvatarUrl { get; set; }
		public int FollowersCount { get; set; }
		public int FollowingCount { get; set; }
		public bool IsFollowing { get; set; } // Is the current user following this profile?
		public DateTime CreatedAt { get; set; }
	}
}
