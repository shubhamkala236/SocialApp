using InteractionService.Infrastructure.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;

namespace InteractionService.Infrastructure.Consumers
{
	public class PostDeletedConsumer : IConsumer<PostDeletedEvent>
	{
		private readonly InteractionDbContext _context;

		public PostDeletedConsumer(InteractionDbContext context)
		{
			_context = context;
		}

		public async Task Consume(ConsumeContext<PostDeletedEvent> context)
		{
			var postId = context.Message.PostId;

			// Remove all likes for deleted post
			var likes = await _context.PostLikes
				.Where(l => l.PostId == postId)
				.ToListAsync();

			// Remove all saves for deleted post
			var saves = await _context.SavedPosts
				.Where(s => s.PostId == postId)
				.ToListAsync();

			_context.PostLikes.RemoveRange(likes);
			_context.SavedPosts.RemoveRange(saves);

			await _context.SaveChangesAsync();

			Console.WriteLine($"✅ Cleaned {likes.Count} likes and {saves.Count} saves for deleted post: {postId}");
		}
	}
}
