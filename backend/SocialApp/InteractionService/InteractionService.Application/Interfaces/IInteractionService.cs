using System;
using System.Collections.Generic;
using System.Text;
using InteractionService.Application.DTO;

namespace InteractionService.Application.Interfaces
{
	public interface IInteractionService
	{
		// ── Likes ─────────────────────────────────────
		Task<LikeResultDto> ToggleLikeAsync(Guid postId, Guid userId, string username);
		Task<int> GetLikesCountAsync(Guid postId);
		Task<bool> IsLikedByUserAsync(Guid postId, Guid userId);

		// ── Saves ─────────────────────────────────────
		Task<bool> SavePostAsync(Guid userId, SavePostDto dto);
		Task<bool> UnsavePostAsync(Guid postId, Guid userId);
		Task<bool> IsPostSavedAsync(Guid postId, Guid userId);
		Task<IEnumerable<SavedPostDto>> GetSavedPostsAsync(Guid userId);

		// ── Combined ──────────────────────────────────
		Task<PostInteractionDto> GetPostInteractionsAsync(Guid postId, Guid? userId);
		Task<IEnumerable<PostInteractionDto>> GetPostsInteractionsAsync(
			IEnumerable<Guid> postIds, Guid? userId);
	}

}
