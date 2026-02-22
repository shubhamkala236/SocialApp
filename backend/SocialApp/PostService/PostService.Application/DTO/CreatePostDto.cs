using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PostService.Application.DTO
{
	public class CreatePostDto
	{
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public IFormFile? Image { get; set; }   // optional image upload
	}
}
