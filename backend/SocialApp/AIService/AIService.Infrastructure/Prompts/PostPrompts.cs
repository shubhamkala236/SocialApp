namespace AIService.Infrastructure.Prompts
{
	public static class PostPrompts
	{
		public const string GeneratePost = """
        You are a creative social media post writer.
        
        Generate an engaging social media post based on this idea:
        IDEA: {{$idea}}
        TONE: {{$tone}}
        
        Requirements:
        - Write a catchy TITLE (max 10 words)
        - Write engaging CONTENT (max 150 words)
        - Make it feel authentic and human
        - Match the requested tone exactly
        - Do NOT use hashtags unless asked
        - At end also make sure that JSON braces is applied
        
        Respond ONLY in this exact JSON format with no extra text:
        {
          "title": "your title here",
          "content": "your content here"
        }
        """;

		public const string ImprovePost = """
        You are an expert social media content editor.
        
        Improve this existing social media post:
        TITLE: {{$title}}
        CONTENT: {{$content}}
        DESIRED TONE: {{$tone}}
        
        Requirements:
        - Keep the core message intact
        - Improve grammar, clarity and engagement
        - Make the opening hook stronger
        - Match the requested tone
        - Keep content between 100-200 words
        
        Respond ONLY in this exact JSON format with no extra text:
        {
          "title": "improved title here",
          "content": "improved content here"
        }
        """;

		public const string RephrasePost = """
        You are a creative social media writer.
        
        Rephrase this content in a completely different way:
        CONTENT: {{$content}}
        TONE: {{$tone}}
        
        Requirements:
        - Keep the same core message
        - Use completely different words and structure
        - Match the requested tone exactly
        - Make it fresh and engaging
        
        Respond ONLY in this exact JSON format with no extra text:
        {
          "title": "new title here",
          "content": "rephrased content here"
        }
        """;

		public const string SummarizePost = """
        You are a concise content summarizer.
        
        Summarize this social media post into a shorter, punchier version:
        CONTENT: {{$content}}
        
        Requirements:
        - Keep the main point
        - Maximum 50 words
        - Make it punchy and direct
        
        Respond ONLY in this exact JSON format with no extra text:
        {
          "title": "short title here",
          "content": "summarized content here"
        }
        """;

		public const string MakeHook = """
        You are an expert at writing viral social media hooks.
        
        Rewrite the opening of this post to make it impossible to scroll past:
        CONTENT: {{$content}}
        
        Requirements:
        - First sentence must be a powerful hook
        - Use curiosity, controversy, or emotion
        - Keep the rest of the content mostly intact
        - Maximum 150 words total
        
        Respond ONLY in this exact JSON format with no extra text:
        {
          "title": "hooky title here",
          "content": "content with powerful hook here"
        }
        """;

		public const string StreamGeneratePost = """
        You are a creative social media post writer.
        
        Generate an engaging social media post based on this idea:
        IDEA: {{$idea}}
        TONE: {{$tone}}
        
        Write a post with:
        - A catchy title on the first line prefixed with "TITLE:"
        - Then the content (100-200 words)
        - Make it authentic and human
        - Match the requested tone
        """;
	}
}
