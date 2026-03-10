using System.Text;
using AIService.Application.DTOs;
using AIService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIService.API.Controllers
{
	[ApiController]
	[Route("api/ai/posts")]
	[Authorize]
	public class PostAssistantController : ControllerBase
	{
		private readonly IPostAssistantService _service;
		private readonly ILogger<PostAssistantController> _logger;

		public PostAssistantController(
			IPostAssistantService service,
			ILogger<PostAssistantController> logger)
		{
			_service = service;
			_logger = logger;
		}

		// POST api/ai/posts/generate
		[HttpPost("generate")]
		public async Task<IActionResult> GeneratePost([FromBody] GeneratePostRequestDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Idea))
				return BadRequest(new { message = "Idea is required." });

			var result = await _service.GeneratePostAsync(dto);
			return Ok(result);
		}

		// POST api/ai/posts/improve
		[HttpPost("improve")]
		public async Task<IActionResult> ImprovePost([FromBody] ImprovePostRequestDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Content))
				return BadRequest(new { message = "Content is required." });

			var result = await _service.ImprovePostAsync(dto);
			return Ok(result);
		}

		// POST api/ai/posts/rephrase
		[HttpPost("rephrase")]
		public async Task<IActionResult> RephrasePost([FromBody] RephrasePostRequestDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Content))
				return BadRequest(new { message = "Content is required." });

			var result = await _service.RephrasePostAsync(dto);
			return Ok(result);
		}

		// POST api/ai/posts/summarize
		[HttpPost("summarize")]
		public async Task<IActionResult> SummarizePost([FromBody] string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				return BadRequest(new { message = "Content is required." });

			var result = await _service.SummarizePostAsync(content);
			return Ok(result);
		}

		// POST api/ai/posts/hook
		[HttpPost("hook")]
		public async Task<IActionResult> MakeHook([FromBody] string content)
		{
			if (string.IsNullOrWhiteSpace(content))
				return BadRequest(new { message = "Content is required." });

			var result = await _service.MakeHookAsync(content);
			return Ok(result);
		}

		// GET api/ai/posts/generate/stream
		// Server-Sent Events for streaming response
		[HttpPost("generate/stream")]
		public async Task StreamGeneratePost([FromBody] GeneratePostRequestDto dto)
		{
			Response.Headers.Add("Content-Type", "text/event-stream");
			Response.Headers.Add("Cache-Control", "no-cache");
			Response.Headers.Add("X-Accel-Buffering", "no");

			await foreach (var chunk in _service.StreamGeneratePostAsync(dto,HttpContext.RequestAborted))
			{
				var data = $"data: {chunk}\n\n";
				await Response.Body.WriteAsync(
					Encoding.UTF8.GetBytes(data));
				await Response.Body.FlushAsync();
			}
		}
	}
}
