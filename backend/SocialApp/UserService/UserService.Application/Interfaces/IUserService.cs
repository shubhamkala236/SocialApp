using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.DTO;

namespace UserService.Application.Interfaces
{
	public interface IUserService
	{
		Task<UserProfileDto?> GetProfileAsync(Guid userId, Guid? currentUserId);
		Task<UserProfileDto> GetOrCreateProfileAsync(Guid userId, string username, string email);
		Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
		Task<IEnumerable<FollowDto>> GetFollowersAsync(Guid userId);
		Task<IEnumerable<FollowDto>> GetFollowingAsync(Guid userId);
		Task<bool> FollowUserAsync(Guid followerId, Guid followingId);
		Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);
		Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
	}
}
