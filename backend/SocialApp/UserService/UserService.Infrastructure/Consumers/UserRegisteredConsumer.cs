using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;
using UserService.Domain;
using UserService.Infrastructure.Context;

namespace UserService.Infrastructure.Consumers
{
	public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
	{
		private readonly UserDbContext _context;

		public UserRegisteredConsumer(UserDbContext context)
		{
			_context = context;
		}

		public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
		{
			var msg = context.Message;

			// Check if profile already exists
			var exists = await _context.UserProfiles
				.AnyAsync(u => u.UserId == msg.UserId);

			if (exists) return;

			// Auto-create profile when user registers
			_context.UserProfiles.Add(new UserProfile
			{
				UserId = msg.UserId,
				Username = msg.Username,
				Email = msg.Email
			});

			await _context.SaveChangesAsync();

			Console.WriteLine($"✅ Profile created for user: {msg.Username}");
		}
	}
}