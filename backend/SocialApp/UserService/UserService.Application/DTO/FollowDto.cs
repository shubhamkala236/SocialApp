using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.DTO
{
	public class FollowDto
	{
		public Guid UserId { get; set; }
		public string Username { get; set; } = string.Empty;
		public string? AvatarUrl { get; set; }
	}

}
