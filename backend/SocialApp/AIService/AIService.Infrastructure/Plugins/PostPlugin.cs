using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIService.Infrastructure.Plugins
{
	public class PostPlugin
	{
		[KernelFunction("generate_post")]
		[Description("Generate a social media post from a rough idea")]
		public string GeneratePost([Description("The rough idea or topic")] string idea, [Description("The tone of the post")] string tone)
		{
			// This is a plugin definition — actual execution is via SK prompts
			return $"Generating post about: {idea} with tone: {tone}";
		}

		[KernelFunction("improve_post")]
		[Description("Improve an existing social media post")]
		public string ImprovePost([Description("The existing post content")] string content, [Description("The tone to use")] string tone)
		{
			return $"Improving post with tone: {tone}";
		}
	}
}
