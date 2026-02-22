using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.DTO
{
	public class AuthResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public Guid UserId { get; set; }
	}
}
