// UserService.API/Controllers/UsersController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}

	private Guid CurrentUserId => Guid.Parse(
		User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	// GET api/users/{userId}
	[HttpGet("{userId:guid}")]
	public async Task<IActionResult> GetProfile(Guid userId)
	{
		Guid? currentUserId = User.Identity?.IsAuthenticated == true
			? CurrentUserId : null;

		var profile = await _userService.GetProfileAsync(userId, currentUserId);
		return profile is null ? NotFound() : Ok(profile);
	}

	// POST api/users/sync
	// Called automatically after login to create/get profile
	[HttpPost("sync")]
	[Authorize]
	public async Task<IActionResult> SyncProfile()
	{
		var userId = CurrentUserId;
		var username = User.FindFirstValue(ClaimTypes.Name)!;
		var email = User.FindFirstValue(ClaimTypes.Email)!;

		var profile = await _userService.GetOrCreateProfileAsync(userId, username, email);
		return Ok(profile);
	}

	// PUT api/users/profile
	[HttpPut("profile")]
	[Authorize]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
	{
		var profile = await _userService.UpdateProfileAsync(CurrentUserId, dto);
		return profile is null ? NotFound() : Ok(profile);
	}

	// GET api/users/{userId}/followers
	[HttpGet("{userId:guid}/followers")]
	public async Task<IActionResult> GetFollowers(Guid userId)
	{
		var followers = await _userService.GetFollowersAsync(userId);
		return Ok(followers);
	}

	// GET api/users/{userId}/following
	[HttpGet("{userId:guid}/following")]
	public async Task<IActionResult> GetFollowing(Guid userId)
	{
		var following = await _userService.GetFollowingAsync(userId);
		return Ok(following);
	}

	// POST api/users/{userId}/follow
	[HttpPost("{userId:guid}/follow")]
	[Authorize]
	public async Task<IActionResult> Follow(Guid userId)
	{
		if (userId == CurrentUserId)
			return BadRequest(new { message = "You cannot follow yourself." });

		var result = await _userService.FollowUserAsync(CurrentUserId, userId);
		return result
			? Ok(new { message = "Followed successfully." })
			: BadRequest(new { message = "Already following or user not found." });
	}

	// DELETE api/users/{userId}/follow
	[HttpDelete("{userId:guid}/follow")]
	[Authorize]
	public async Task<IActionResult> Unfollow(Guid userId)
	{
		var result = await _userService.UnfollowUserAsync(CurrentUserId, userId);
		return result
			? Ok(new { message = "Unfollowed successfully." })
			: NotFound(new { message = "Follow relationship not found." });
	}

	// GET api/users/{userId}/is-following
	[HttpGet("{userId:guid}/is-following")]
	[Authorize]
	public async Task<IActionResult> IsFollowing(Guid userId)
	{
		var result = await _userService.IsFollowingAsync(CurrentUserId, userId);
		return Ok(new { isFollowing = result });
	}
}