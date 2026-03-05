using System;
using System.Collections.Generic;
using System.Text;
using InteractionService.Infrastructure.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;

namespace InteractionService.Infrastructure.Consumers
{
	public class UserDeletedConsumer : IConsumer<UserDeletedEvent>
	{
		private readonly InteractionDbContext _context;

		public UserDeletedConsumer(InteractionDbContext context)
		{
			_context = context;
		}

		public async Task Consume(ConsumeContext<UserDeletedEvent> context)
		{
			var userId = context.Message.UserId;

			// Remove all likes by deleted user
			var likes = await _context.PostLikes
				.Where(l => l.UserId == userId)
				.ToListAsync();

			// Remove all saves by deleted user
			var saves = await _context.SavedPosts
				.Where(s => s.UserId == userId)
				.ToListAsync();

			_context.PostLikes.RemoveRange(likes);
			_context.SavedPosts.RemoveRange(saves);

			await _context.SaveChangesAsync();

			Console.WriteLine($"✅ Cleaned {likes.Count} likes and {saves.Count} saves for deleted user: {userId}");
		}
	}
}
