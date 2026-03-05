using MassTransit;
using Microsoft.EntityFrameworkCore;
using PostService.Application.DTO;
using PostService.Application.Interfaces;
using PostService.Domain;
using PostService.Infrastructure.Context;
using Shared.Contracts.Events;

namespace PostService.Infrastructure.Service
{
	public class PostServices : IPostService
	{
		private readonly PostDbContext _context;
		private readonly IPhotoService _photoService;
		private readonly IPublishEndpoint _publishEndpoint;

		public PostServices(PostDbContext context, IPhotoService photoService, IPublishEndpoint publishEndpoint)
		{
			_context = context;
			_photoService = photoService;
			_publishEndpoint = publishEndpoint;
		}

		public async Task<IEnumerable<PostResponseDto>> GetAllPostsAsync()
		{
			return await _context.Posts
				.OrderByDescending(p => p.CreatedAt)
				.Select(p => MapToDto(p))
				.ToListAsync();
		}

		public async Task<PostResponseDto?> GetPostByIdAsync(Guid id)
		{
			var post = await _context.Posts.FindAsync(id);
			return post is null ? null : MapToDto(post);
		}

		public async Task<IEnumerable<PostResponseDto>> GetPostsByUserIdAsync(Guid userId)
		{
			return await _context.Posts
				.Where(p => p.UserId == userId)
				.OrderByDescending(p => p.CreatedAt)
				.Select(p => MapToDto(p))
				.ToListAsync();
		}

		public async Task<PostResponseDto> CreatePostAsync(CreatePostDto dto, Guid userId, string username, string? userAvatarUrl)
		{
			var post = new Post
			{
				Title = dto.Title,
				Content = dto.Content,
				UserId = userId,
				Username = username,
				UserAvatarUrl = userAvatarUrl
			};

			if (dto.Image is not null)
			{
				var uploadResult = await _photoService.UploadPhotoAsync(dto.Image);
				if (uploadResult is not null)
				{
					post.ImageUrl = uploadResult.Url;
					post.ImagePublicId = uploadResult.PublicId;
				}
			}

			_context.Posts.Add(post);
			await _context.SaveChangesAsync();

			await _publishEndpoint.Publish(new PostCreatedEvent
			{
				PostId = post.Id,
				UserId = userId,
				Username = username,
				Title = post.Title,
				CreatedAt = post.CreatedAt
			});

			return MapToDto(post);
		}

		public async Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid userId)
		{
			var post = await _context.Posts.FindAsync(id);

			if (post is null) return null;

			if (post.UserId != userId)
				throw new UnauthorizedAccessException("You can only edit your own posts.");

			post.Title = dto.Title;
			post.Content = dto.Content;
			post.UpdatedAt = DateTime.UtcNow;

			// Handle image update
			if (dto.Image is not null)
			{
				// Delete old image from Cloudinary first
				if (!string.IsNullOrEmpty(post.ImagePublicId))
					await _photoService.DeletePhotoAsync(post.ImagePublicId);

				var uploadResult = await _photoService.UploadPhotoAsync(dto.Image);
				if (uploadResult is not null)
				{
					post.ImageUrl = uploadResult.Url;
					post.ImagePublicId = uploadResult.PublicId;
				}
			}

			await _context.SaveChangesAsync();
			return MapToDto(post);
		}

		public async Task<bool> DeletePostAsync(Guid id, Guid userId)
		{
			var post = await _context.Posts.FindAsync(id);

			if (post is null) return false;

			if (post.UserId != userId)
				throw new UnauthorizedAccessException("You can only delete your own posts.");

			// Delete image from Cloudinary when post is deleted
			if (!string.IsNullOrEmpty(post.ImagePublicId))
				await _photoService.DeletePhotoAsync(post.ImagePublicId);

			_context.Posts.Remove(post);
			await _context.SaveChangesAsync();

			// ── Publish PostDeletedEvent ───────────────────
			await _publishEndpoint.Publish(new PostDeletedEvent
			{
				PostId = id,
				UserId = userId,
				DeletedAt = DateTime.UtcNow
			});

			return true;
		}

		private static PostResponseDto MapToDto(Post post) => new()
		{
			Id = post.Id,
			Title = post.Title,
			Content = post.Content,
			UserId = post.UserId,
			Username = post.Username,
			UserAvatarUrl = post.UserAvatarUrl,
			ImageUrl = post.ImageUrl,
			CreatedAt = post.CreatedAt,
			UpdatedAt = post.UpdatedAt
		};
	}
}
