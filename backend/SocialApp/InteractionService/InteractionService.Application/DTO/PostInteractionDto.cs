using System;
using System.Collections.Generic;
using System.Text;

namespace InteractionService.Application.DTO
{
	public class PostInteractionDto
	{
		public Guid PostId { get; set; }
		public int LikesCount { get; set; }
		public bool IsLiked { get; set; }
		public bool IsSaved { get; set; }
	}

}
