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
						Temperature = (float?)0.8,
						ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
						Temperature = (float?)0.6,
						ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
						Temperature = (float?)0.9,
						ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
						Temperature = (float?)0.4,
						ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
						Temperature = (float?)0.85,
						ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
					Temperature = (float?)0.8,
					ExtensionData = new Dictionary<string, object> { ["num_predict"] = 500 } // Ollama's parameter for max tokens
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
				_logger.LogInformation("Raw AI response: {Raw}", raw);

				// ✅ Strip markdown code blocks
				raw = raw.Trim();
				if (raw.StartsWith("```json"))
					raw = raw.Substring(7);
				if (raw.StartsWith("```"))
					raw = raw.Substring(3);
				if (raw.EndsWith("```"))
					raw = raw.Substring(0, raw.Length - 3);

				raw = raw.Trim();

				// ✅ Extract JSON object
				var start = raw.IndexOf('{');
				if (start == -1)
					throw new Exception("No JSON object found");

				var json = raw.Substring(start);

				// ✅ Fix unclosed JSON — count braces and close if needed
				json = FixUnclosedJson(json);

				_logger.LogInformation("Fixed JSON: {Json}", json);

				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					AllowTrailingCommas = true,
				};

				var parsed = JsonSerializer.Deserialize<PostAssistantResponseDto>(json, options);

				if (parsed is null)
					throw new Exception("Deserialization returned null");

				return new PostAssistantResponseDto
				{
					Title = parsed.Title?.Trim() ?? string.Empty,
					Content = parsed.Content?.Trim() ?? string.Empty,
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
					ActionTaken = actionTaken + " - Failed"
				};
			}
		}

		private static string FixUnclosedJson(string json)
		{
			// Count open/close braces
			int openBraces = json.Count(c => c == '{');
			int closeBraces = json.Count(c => c == '}');
			int missing = openBraces - closeBraces;

			if (missing <= 0) return json;

			// ✅ Check if last string value is unclosed (missing closing quote)
			var trimmed = json.TrimEnd();

			// If last char is not a quote or brace, the string value is cut off
			if (!trimmed.EndsWith("\"") && !trimmed.EndsWith("}"))
			{
				// Find last complete sentence or word — close the string cleanly
				// Trim any trailing partial word/punctuation that looks incomplete
				trimmed = trimmed.TrimEnd(',', ' ', '\n', '\r');
				trimmed += "\"";  // close the open string
			}

			// Close any missing braces
			for (int i = 0; i < missing; i++)
				trimmed += "}";

			return trimmed;
		}

		private PostAssistantResponseDto TryRegexExtract(string raw, string actionTaken)
		{
			try
			{
				var titleMatch = System.Text.RegularExpressions.Regex.Match(
					raw, "\"title\"\\s*:\\s*\"([^\"]+)\"");
				var contentMatch = System.Text.RegularExpressions.Regex.Match(
					raw, "\"content\"\\s*:\\s*\"([\\s\\S]+?)\"(?:\\s*[,}])");

				// If content regex fails (unclosed), grab everything after "content":
				if (!contentMatch.Success)
				{
					contentMatch = System.Text.RegularExpressions.Regex.Match(
						raw, "\"content\"\\s*:\\s*\"([\\s\\S]+)");
				}

				var title = titleMatch.Success
					? titleMatch.Groups[1].Value.Trim()
					: "AI Generated Post";

				var content = contentMatch.Success
					? contentMatch.Groups[1].Value
						.Trim()
						.TrimEnd('"', '}', ' ', '\n')
						.Trim()
					: raw;

				_logger.LogInformation(
					"Regex extraction succeeded — title: {Title}", title);

				return new PostAssistantResponseDto
				{
					Title = title,
					Content = content,
					ActionTaken = actionTaken
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Regex extraction also failed");
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
