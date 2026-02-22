using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTO;
using PostService.Application.Interfaces;

namespace PostService.API.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PostsController : ControllerBase
	{
		private readonly IPostService _postService;
		public PostsController(IPostService postService) => _postService = postService;

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAll() =>
			Ok(await _postService.GetAllPostsAsync());

		[HttpGet("{id:guid}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetById(Guid id)
		{
			var post = await _postService.GetPostByIdAsync(id);
			return post is null ? NotFound() : Ok(post);
		}

		[HttpGet("user/{userId:guid}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetByUser(Guid userId) =>
			Ok(await _postService.GetPostsByUserIdAsync(userId));

		[HttpPost]
		[Consumes("multipart/form-data")]   // ← important for file upload
		public async Task<IActionResult> Create([FromForm] CreatePostDto dto)
		{
			var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var username = User.FindFirstValue(ClaimTypes.Name)!;

			var post = await _postService.CreatePostAsync(dto, userId, username);
			return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
		}

		[HttpPut("{id:guid}")]
		[Consumes("multipart/form-data")]   // ← important for file upload
		public async Task<IActionResult> Update(Guid id, [FromForm] UpdatePostDto dto)
		{
			try
			{
				var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
				var post = await _postService.UpdatePostAsync(id, dto, userId);
				return post is null ? NotFound() : Ok(post);
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
		}

		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
				var result = await _postService.DeletePostAsync(id, userId);
				return result ? NoContent() : NotFound();
			}
			catch (UnauthorizedAccessException)
			{
				return Forbid();
			}
		}
	}
}