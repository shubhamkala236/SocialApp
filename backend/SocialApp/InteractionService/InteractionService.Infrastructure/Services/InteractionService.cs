using InteractionService.Application.DTO;
using InteractionService.Application.Interfaces;
using InteractionService.Domain.Entity;
using InteractionService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InteractionService.Infrastructure.Services
{
	public class InteractionService : IInteractionService
	{
		private readonly InteractionDbContext _context;

		public InteractionService(InteractionDbContext context)
		{
			_context = context;
		}

		// ── Likes ─────────────────────────────────────────────────────────────

		public async Task<LikeResultDto> ToggleLikeAsync(
			Guid postId, Guid userId, string username)
		{
			var existing = await _context.PostLikes
				.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

			if (existing is not null)
			{
				// Unlike
				_context.PostLikes.Remove(existing);
			}
			else
			{
				// Like
				_context.PostLikes.Add(new PostLike
				{
					PostId = postId,
					UserId = userId,
					Username = username
				});
			}

			await _context.SaveChangesAsync();

			var count = await _context.PostLikes.CountAsync(l => l.PostId == postId);
			var isLiked = existing is null; // if it existed before, we unliked it

			return new LikeResultDto { IsLiked = isLiked, LikesCount = count };
		}

		public async Task<int> GetLikesCountAsync(Guid postId)
		{
			return await _context.PostLikes.CountAsync(l => l.PostId == postId);
		}

		public async Task<bool> IsLikedByUserAsync(Guid postId, Guid userId)
		{
			return await _context.PostLikes
				.AnyAsync(l => l.PostId == postId && l.UserId == userId);
		}

		// ── Saves ─────────────────────────────────────────────────────────────

		public async Task<bool> SavePostAsync(Guid userId, SavePostDto dto)
		{
			var alreadySaved = await _context.SavedPosts
				.AnyAsync(s => s.PostId == dto.PostId && s.UserId == userId);

			if (alreadySaved) return false;

			_context.SavedPosts.Add(new SavedPost
			{
				PostId = dto.PostId,
				UserId = userId,
				PostTitle = dto.PostTitle,
				PostContent = dto.PostContent,
				PostUsername = dto.PostUsername,
				PostImageUrl = dto.PostImageUrl,
				PostCreatedAt = dto.PostCreatedAt
			});

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UnsavePostAsync(Guid postId, Guid userId)
		{
			var saved = await _context.SavedPosts
				.FirstOrDefaultAsync(s => s.PostId == postId && s.UserId == userId);

			if (saved is null) return false;

			_context.SavedPosts.Remove(saved);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> IsPostSavedAsync(Guid postId, Guid userId)
		{
			return await _context.SavedPosts
				.AnyAsync(s => s.PostId == postId && s.UserId == userId);
		}

		public async Task<IEnumerable<SavedPostDto>> GetSavedPostsAsync(Guid userId)
		{
			return await _context.SavedPosts
				.Where(s => s.UserId == userId)
				.OrderByDescending(s => s.SavedAt)
				.Select(s => new SavedPostDto
				{
					PostId = s.PostId,
					PostTitle = s.PostTitle,
					PostContent = s.PostContent,
					PostUsername = s.PostUsername,
					PostImageUrl = s.PostImageUrl,
					PostCreatedAt = s.PostCreatedAt,
					SavedAt = s.SavedAt
				})
				.ToListAsync();
		}

		// ── Combined ──────────────────────────────────────────────────────────

		public async Task<PostInteractionDto> GetPostInteractionsAsync(
			Guid postId, Guid? userId)
		{
			var likesCount = await _context.PostLikes.CountAsync(l => l.PostId == postId);
			var isLiked = userId.HasValue &&
							 await _context.PostLikes
								 .AnyAsync(l => l.PostId == postId && l.UserId == userId.Value);
			var isSaved = userId.HasValue &&
							 await _context.SavedPosts
								 .AnyAsync(s => s.PostId == postId && s.UserId == userId.Value);

			return new PostInteractionDto
			{
				PostId = postId,
				LikesCount = likesCount,
				IsLiked = isLiked,
				IsSaved = isSaved
			};
		}

		public async Task<IEnumerable<PostInteractionDto>> GetPostsInteractionsAsync(
			IEnumerable<Guid> postIds, Guid? userId)
		{
			var postIdsList = postIds.ToList();

			// Batch fetch all likes and saves for given post IDs
			var likeCounts = await _context.PostLikes
				.Where(l => postIdsList.Contains(l.PostId))
				.GroupBy(l => l.PostId)
				.Select(g => new { PostId = g.Key, Count = g.Count() })
				.ToListAsync();

			var likedByUser = userId.HasValue
				? await _context.PostLikes
					.Where(l => postIdsList.Contains(l.PostId) && l.UserId == userId.Value)
					.Select(l => l.PostId)
					.ToListAsync()
				: new List<Guid>();

			var savedByUser = userId.HasValue
				? await _context.SavedPosts
					.Where(s => postIdsList.Contains(s.PostId) && s.UserId == userId.Value)
					.Select(s => s.PostId)
					.ToListAsync()
				: new List<Guid>();

			return postIdsList.Select(postId => new PostInteractionDto
			{
				PostId = postId,
				LikesCount = likeCounts.FirstOrDefault(l => l.PostId == postId)?.Count ?? 0,
				IsLiked = likedByUser.Contains(postId),
				IsSaved = savedByUser.Contains(postId)
			});
		}
	}

}
