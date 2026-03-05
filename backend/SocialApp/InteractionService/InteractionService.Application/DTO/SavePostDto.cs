using System;
using System.Collections.Generic;
using System.Text;

namespace InteractionService.Application.DTO
{
	public class SavePostDto
	{
		public Guid PostId { get; set; }
		public string PostTitle { get; set; } = string.Empty;
		public string PostContent { get; set; } = string.Empty;
		public string PostUsername { get; set; } = string.Empty;
		public string? PostImageUrl { get; set; }
		public DateTime PostCreatedAt { get; set; }
	}

	public class SavedPostDto
	{
		public Guid PostId { get; set; }
		public string PostTitle { get; set; } = string.Empty;
		public string PostContent { get; set; } = string.Empty;
		public string PostUsername { get; set; } = string.Empty;
		public string? PostImageUrl { get; set; }
		public DateTime PostCreatedAt { get; set; }
		public DateTime SavedAt { get; set; }
	}

}
