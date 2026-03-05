using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;
using UserService.Infrastructure.Context;

namespace UserService.Infrastructure.Consumers
{
	public class UserDeletedConsumer : IConsumer<UserDeletedEvent>
	{
		private readonly UserDbContext _context;

		public UserDeletedConsumer(UserDbContext context)
		{
			_context = context;
		}

		public async Task Consume(ConsumeContext<UserDeletedEvent> context)
		{
			var msg = context.Message;

			var profile = await _context.UserProfiles
				.Include(u => u.Followers)
				.Include(u => u.Following)
				.FirstOrDefaultAsync(u => u.UserId == msg.UserId);

			if (profile is null) return;

			// Remove all follows
			_context.Follows.RemoveRange(profile.Followers);
			_context.Follows.RemoveRange(profile.Following);

			// Remove profile
			_context.UserProfiles.Remove(profile);

			await _context.SaveChangesAsync();

			Console.WriteLine($"✅ Profile deleted for user: {msg.UserId}");
		}
	}
}
