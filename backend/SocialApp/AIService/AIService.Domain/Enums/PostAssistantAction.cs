using System;
using System.Collections.Generic;
using System.Text;

namespace AIService.Domain.Enums
{
	public enum PostAssistantAction
	{
		Generate,   // Generate full post from rough idea
		Improve,    // Improve existing post
		Rephrase,   // Rephrase with different tone
		Summarize,  // Summarize long post
		MakeHook    // Make opening hook more engaging
	}

	public enum PostTone
	{
		Professional,
		Casual,
		Funny,
		Inspirational,
		Storytelling
	}
}
