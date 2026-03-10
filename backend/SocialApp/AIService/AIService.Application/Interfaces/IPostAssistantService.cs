using AIService.Application.DTOs;

namespace AIService.Application.Interfaces
{
	public interface IPostAssistantService
	{
		Task<PostAssistantResponseDto> GeneratePostAsync(GeneratePostRequestDto dto);
		Task<PostAssistantResponseDto> ImprovePostAsync(ImprovePostRequestDto dto);
		Task<PostAssistantResponseDto> RephrasePostAsync(RephrasePostRequestDto dto);
		Task<PostAssistantResponseDto> SummarizePostAsync(string content);
		Task<PostAssistantResponseDto> MakeHookAsync(string content);
		IAsyncEnumerable<string> StreamGeneratePostAsync(GeneratePostRequestDto dto, CancellationToken cancellationToken = default);
	}
}
