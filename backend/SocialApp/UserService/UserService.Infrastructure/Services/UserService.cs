// UserService.Infrastructure/Services/UserService.cs
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;
using UserService.Application.DTO;
using UserService.Application.Interfaces;
using UserService.Domain;
using UserService.Infrastructure.Context;

namespace UserService.Infrastructure.Services;

public class UserService : IUserService
{
	private readonly UserDbContext _context;
	private readonly IPhotoService _photoService;
	private readonly IPublishEndpoint _publishEndpoint;

	public UserService(UserDbContext context, IPhotoService photoService, IPublishEndpoint publishEndpoint)
	{
		_context = context;
		_photoService = photoService;
		_publishEndpoint = publishEndpoint;
	}

	public async Task<UserProfileDto?> GetProfileAsync(Guid userId, Guid? currentUserId)
	{
		var profile = await _context.UserProfiles
			.Include(u => u.Followers)
			.Include(u => u.Following)
			.FirstOrDefaultAsync(u => u.UserId == userId);

		if (profile is null) return null;

		return MapToDto(profile, currentUserId);
	}

	public async Task<UserProfileDto> GetOrCreateProfileAsync(
		Guid userId, string username, string email)
	{
		var profile = await _context.UserProfiles
			.FirstOrDefaultAsync(u => u.UserId == userId);

		if (profile is not null) return MapToDto(profile, null);

		// Auto-create profile on first login
		profile = new UserProfile
		{
			UserId = userId,
			Username = username,
			Email = email
		};

		_context.UserProfiles.Add(profile);
		await _context.SaveChangesAsync();

		return MapToDto(profile, null);
	}

	public async Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
	{
		var profile = await _context.UserProfiles
			.FirstOrDefaultAsync(u => u.UserId == userId);

		if (profile is null) return null;

		if (dto.Bio is not null)
			profile.Bio = dto.Bio;

		// Handle avatar upload
		if (dto.Avatar is not null)
		{
			// Delete old avatar
			if (!string.IsNullOrEmpty(profile.AvatarPublicId))
				await _photoService.DeletePhotoAsync(profile.AvatarPublicId);

			var result = await _photoService.UploadPhotoAsync(dto.Avatar);
			if (result is not null)
			{
				profile.AvatarUrl = result.Url;
				profile.AvatarPublicId = result.PublicId;

				// ✅ Publish so AuthService can update JWT avatar
				await _publishEndpoint.Publish(new UserAvatarUpdatedEvent
				{
					UserId = userId,
					AvatarUrl = result.Url
				});
			}
		}

		profile.UpdatedAt = DateTime.UtcNow;
		await _context.SaveChangesAsync();

		return MapToDto(profile, userId);
	}

	public async Task<IEnumerable<FollowDto>> GetFollowersAsync(Guid userId)
	{
		return await _context.Follows
			.Where(f => f.FollowingId == userId)
			.Include(f => f.Follower)
			.Select(f => new FollowDto
			{
				UserId = f.Follower.UserId,
				Username = f.Follower.Username,
				AvatarUrl = f.Follower.AvatarUrl
			})
			.ToListAsync();
	}

	public async Task<IEnumerable<FollowDto>> GetFollowingAsync(Guid userId)
	{
		return await _context.Follows
			.Where(f => f.FollowerId == userId)
			.Include(f => f.Following)
			.Select(f => new FollowDto
			{
				UserId = f.Following.UserId,
				Username = f.Following.Username,
				AvatarUrl = f.Following.AvatarUrl
			})
			.ToListAsync();
	}

	public async Task<bool> FollowUserAsync(Guid followerId, Guid followingId)
	{
		if (followerId == followingId) return false;

		var exists = await _context.Follows
			.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
		if (exists) return false;

		var follower = await _context.UserProfiles
			.FirstOrDefaultAsync(u => u.UserId == followerId);
		var following = await _context.UserProfiles
			.FirstOrDefaultAsync(u => u.UserId == followingId);

		if (follower is null || following is null) return false;

		_context.Follows.Add(new Follow
		{
			FollowerId = followerId,
			FollowingId = followingId
		});

		await _context.SaveChangesAsync();

		// ── Publish UserFollowedEvent ──────────────────
		await _publishEndpoint.Publish(new UserFollowedEvent
		{
			FollowerId = followerId,
			FollowerUsername = follower.Username,
			FollowingId = followingId,
			FollowingUsername = following.Username,
			FollowedAt = DateTime.UtcNow
		});

		return true;
	}

	public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
	{
		var follow = await _context.Follows
			.FirstOrDefaultAsync(f =>
				f.FollowerId == followerId &&
				f.FollowingId == followingId);

		if (follow is null) return false;

		_context.Follows.Remove(follow);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
	{
		return await _context.Follows
			.AnyAsync(f =>
				f.FollowerId == followerId &&
				f.FollowingId == followingId);
	}

	private static UserProfileDto MapToDto(UserProfile profile, Guid? currentUserId)
	{
		return new UserProfileDto
		{
			UserId = profile.UserId,
			Username = profile.Username,
			Email = profile.Email,
			Bio = profile.Bio,
			AvatarUrl = profile.AvatarUrl,
			FollowersCount = profile.Followers?.Count ?? 0,
			FollowingCount = profile.Following?.Count ?? 0,
			IsFollowing = currentUserId.HasValue &&
							 profile.Followers.Any(f => f.FollowerId == currentUserId.Value),
			CreatedAt = profile.CreatedAt
		};
	}
}