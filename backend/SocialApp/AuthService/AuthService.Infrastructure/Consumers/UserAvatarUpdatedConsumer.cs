using System;
using System.Collections.Generic;
using System.Text;
using AuthService.Infrastructure.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;

namespace AuthService.Infrastructure.Consumers
{
	public class UserAvatarUpdatedConsumer : IConsumer<UserAvatarUpdatedEvent>
	{
		private readonly AuthDbContext _context;

		public UserAvatarUpdatedConsumer(AuthDbContext context)
		{
			_context = context;
		}

		public async Task Consume(ConsumeContext<UserAvatarUpdatedEvent> context)
		{
			var msg = context.Message;

			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == msg.UserId);

			if (user is null) return;

			user.AvatarUrl = msg.AvatarUrl;
			await _context.SaveChangesAsync();

			Console.WriteLine($"✅ Avatar updated for user: {msg.UserId}");
		}
	}
}
