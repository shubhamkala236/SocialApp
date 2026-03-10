using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AIService.Application.DTOs;
using AIService.Application.Interfaces;
using AIService.Infrastructure.Prompts;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace AIService.Infrastructure.Services
{
	public class PostAssistantService : IPostAssistantService
	{
		private readonly Kernel _kernel;
		private readonly ILogger<PostAssistantService> _logger;
		public PostAssistantService(Kernel kernel, ILogger<PostAssistantService> logger)
		{
			_kernel = kernel;
			_logger = logger;
		}
		// ── Generate Post ─────────────────────────────────────────────────────
		public async Task<PostAssistantResponseDto> GeneratePostAsync(GeneratePostRequestDto dto)
		{
			try
			{
				var func = _kernel.CreateFunctionFromPrompt(
					PostPrompts.GeneratePost,
					new OllamaPromptExecutionSettings
					{
						ModelId = "llama3.2",
						Temperature = (float?)0.8
					});

				var result = await _kernel.InvokeAsync(func, new KernelArguments
				{
					["idea"] = dto.Idea,
					["tone"] = dto.Tone.ToString()
				});

				return ParseResponse(result.ToString(), "Generated");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating post");
				throw;
			}
		}

		// ── Improve Post ──────────────────────────────────────────────────────
		public async Task<PostAssistantResponseDto> ImprovePostAsync(ImprovePostRequestDto dto)
		{
			try
			{
				var func = _kernel.CreateFunctionFromPrompt(
					PostPrompts.ImprovePost,
					new OllamaPromptExecutionSettings
					{
						ModelId = "llama3.2",
						Temperature = (float?)0.6
					});

				var result = await _kernel.InvokeAsync(func, new KernelArguments
				{
					["title"] = dto.Title,
					["content"] = dto.Content,
					["tone"] = dto.Tone.ToString()
				});

				return ParseResponse(result.ToString(), "Improved");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error improving post");
				throw;
			}
		}

		// ── Rephrase Post ─────────────────────────────────────────────────────
		public async Task<PostAssistantResponseDto> RephrasePostAsync(RephrasePostRequestDto dto)
		{
			try
			{
				var func = _kernel.CreateFunctionFromPrompt(
					PostPrompts.RephrasePost,
					new OllamaPromptExecutionSettings
					{
						ModelId = "llama3.2",
						Temperature = (float?)0.9
					});

				var result = await _kernel.InvokeAsync(func, new KernelArguments
				{
					["content"] = dto.Content,
					["tone"] = dto.Tone.ToString()
				});

				return ParseResponse(result.ToString(), "Rephrased");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error rephrasing post");
				throw;
			}
		}

		// ── Summarize Post ────────────────────────────────────────────────────
		public async Task<PostAssistantResponseDto> SummarizePostAsync(string content)
		{
			try
			{
				var func = _kernel.CreateFunctionFromPrompt(
					PostPrompts.SummarizePost,
					new OllamaPromptExecutionSettings
					{
						ModelId = "llama3.2",
						Temperature = (float?)0.4
					});

				var result = await _kernel.InvokeAsync(func, new KernelArguments
				{
					["content"] = content
				});

				return ParseResponse(result.ToString(), "Summarized");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error summarizing post");
				throw;
			}
		}

		// ── Make Hook ─────────────────────────────────────────────────────────
		public async Task<PostAssistantResponseDto> MakeHookAsync(string content)
		{
			try
			{
				var func = _kernel.CreateFunctionFromPrompt(
					PostPrompts.MakeHook,
					new OllamaPromptExecutionSettings
					{
						ModelId = "llama3.2",
						Temperature = (float?)0.85
					});

				var result = await _kernel.InvokeAsync(func, new KernelArguments
				{
					["content"] = content
				});

				return ParseResponse(result.ToString(), "Hook Added");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error making hook");
				throw;
			}
		}

		// ── Stream Generate Post ──────────────────────────────────────────────
		public async IAsyncEnumerable<string> StreamGeneratePostAsync(GeneratePostRequestDto dto, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			var func = _kernel.CreateFunctionFromPrompt(
				PostPrompts.StreamGeneratePost,
				new OllamaPromptExecutionSettings
				{
					ModelId = "llama3.2",
					Temperature = (float?)0.8
				});

			await foreach (var chunk in _kernel.InvokeStreamingAsync<string>(
				func,
				new KernelArguments
				{
					["idea"] = dto.Idea,
					["tone"] = dto.Tone.ToString()
				},
				cancellationToken))
			{
				yield return chunk;
			}
		}

		// ── Parse JSON Response ───────────────────────────────────────────────
		private PostAssistantResponseDto ParseResponse(string raw, string actionTaken)
		{
			try
			{
				// Extract JSON from response (model sometimes adds extra text)
				var start = raw.IndexOf('{');
				var end = raw.LastIndexOf('}');

				if (start == -1 || end == -1)
					throw new Exception("No JSON found in response");

				var json = raw.Substring(start, end - start + 1);

				var parsed = JsonSerializer.Deserialize<JsonElement>(json);

				return new PostAssistantResponseDto
				{
					Title = parsed.GetProperty("title").GetString() ?? string.Empty,
					Content = parsed.GetProperty("content").GetString() ?? string.Empty,
					ActionTaken = actionTaken
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to parse AI response: {Raw}", raw);

				// Fallback — return raw response
				return new PostAssistantResponseDto
				{
					Title = "AI Generated Post",
					Content = raw,
					ActionTaken = actionTaken
				};
			}
		}
	}
}
