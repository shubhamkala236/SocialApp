using AIService.Domain.Enums;

namespace AIService.Application.DTOs
{
	public class PostAssistantRequestDto
	{
		public PostAssistantAction Action { get; set; }
		public string Input { get; set; } = string.Empty; // rough idea or existing post
		public PostTone Tone { get; set; } = PostTone.Casual;
	}

	public class PostAssistantResponseDto
	{
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string ActionTaken { get; set; } = string.Empty;
	}

	public class ImprovePostRequestDto
	{
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public PostTone Tone { get; set; } = PostTone.Casual;
	}

	public class GeneratePostRequestDto
	{
		public string Idea { get; set; } = string.Empty;
		public PostTone Tone { get; set; } = PostTone.Casual;
	}

	public class RephrasePostRequestDto
	{
		public string Content { get; set; } = string.Empty;
		public PostTone Tone { get; set; } = PostTone.Casual;
	}
}
