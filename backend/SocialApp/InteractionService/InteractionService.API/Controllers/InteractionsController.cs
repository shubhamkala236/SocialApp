using System.Security.Claims;
using InteractionService.Application.DTO;
using InteractionService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InteractionService.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InteractionsController : ControllerBase
	{
		private readonly IInteractionService _service;

		public InteractionsController(IInteractionService service)
		{
			_service = service;
		}

		private Guid CurrentUserId => Guid.Parse(
			User.FindFirstValue(ClaimTypes.NameIdentifier)!);

		private string CurrentUsername =>
			User.FindFirstValue(ClaimTypes.Name)!;

		// ── Likes ──────────────────────────────────────────

		// POST api/interactions/posts/{postId}/like
		[HttpPost("posts/{postId:guid}/like")]
		[Authorize]
		public async Task<IActionResult> ToggleLike(Guid postId)
		{
			var result = await _service.ToggleLikeAsync(
				postId, CurrentUserId, CurrentUsername);
			return Ok(result);
		}

		// GET api/interactions/posts/{postId}/likes
		[HttpGet("posts/{postId:guid}/likes")]
		public async Task<IActionResult> GetLikesCount(Guid postId)
		{
			var count = await _service.GetLikesCountAsync(postId);
			return Ok(new { likesCount = count });
		}

		// GET api/interactions/posts/{postId}/is-liked
		[HttpGet("posts/{postId:guid}/is-liked")]
		[Authorize]
		public async Task<IActionResult> IsLiked(Guid postId)
		{
			var isLiked = await _service.IsLikedByUserAsync(postId, CurrentUserId);
			return Ok(new { isLiked });
		}

		// ── Saves ──────────────────────────────────────────

		// POST api/interactions/posts/save
		[HttpPost("posts/save")]
		[Authorize]
		public async Task<IActionResult> SavePost([FromBody] SavePostDto dto)
		{
			var result = await _service.SavePostAsync(CurrentUserId, dto);
			return result
				? Ok(new { message = "Post saved." })
				: BadRequest(new { message = "Post already saved." });
		}

		// DELETE api/interactions/posts/{postId}/save
		[HttpDelete("posts/{postId:guid}/save")]
		[Authorize]
		public async Task<IActionResult> UnsavePost(Guid postId)
		{
			var result = await _service.UnsavePostAsync(postId, CurrentUserId);
			return result
				? Ok(new { message = "Post unsaved." })
				: NotFound(new { message = "Saved post not found." });
		}

		// GET api/interactions/posts/saved
		[HttpGet("posts/saved")]
		[Authorize]
		public async Task<IActionResult> GetSavedPosts()
		{
			var saved = await _service.GetSavedPostsAsync(CurrentUserId);
			return Ok(saved);
		}

		// ── Combined ───────────────────────────────────────

		// GET api/interactions/posts/{postId}
		[HttpGet("posts/{postId:guid}")]
		public async Task<IActionResult> GetPostInteractions(Guid postId)
		{
			Guid? currentUserId = User.Identity?.IsAuthenticated == true
				? CurrentUserId : null;

			var result = await _service.GetPostInteractionsAsync(postId, currentUserId);
			return Ok(result);
		}

		// POST api/interactions/posts/batch
		// Send list of postIds, get back all interactions in one call
		[HttpPost("posts/batch")]
		public async Task<IActionResult> GetBatchInteractions([FromBody] List<Guid> postIds)
		{
			Guid? currentUserId = User.Identity?.IsAuthenticated == true
				? CurrentUserId : null;

			var result = await _service.GetPostsInteractionsAsync(postIds, currentUserId);
			return Ok(result);
		}
	}
}