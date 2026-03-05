using Microsoft.AspNetCore.Http;

namespace UserService.Application.DTO
{
	public class UpdateProfileDto
	{
		public string? Bio { get; set; }
		public IFormFile? Avatar { get; set; }
	}
}
