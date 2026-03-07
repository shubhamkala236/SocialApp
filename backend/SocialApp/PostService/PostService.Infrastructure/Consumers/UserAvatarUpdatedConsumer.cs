using MassTransit;
using Microsoft.EntityFrameworkCore;
using PostService.Infrastructure.Context;
using Shared.Contracts.Events;

namespace PostService.Infrastructure.Consumers;

public class UserAvatarUpdatedConsumer : IConsumer<UserAvatarUpdatedEvent>
{
	private readonly PostDbContext _context;

	public UserAvatarUpdatedConsumer(PostDbContext context)
	{
		_context = context;
	}

	public async Task Consume(ConsumeContext<UserAvatarUpdatedEvent> context)
	{
		var msg = context.Message;

		// ✅ Update all posts by this user with new avatar
		var posts = await _context.Posts
			.Where(p => p.UserId == msg.UserId)
			.ToListAsync();

		if (!posts.Any()) return;

		foreach (var post in posts)
			post.UserAvatarUrl = msg.AvatarUrl;

		await _context.SaveChangesAsync();

		Console.WriteLine(
			$"✅ Updated avatar on {posts.Count} posts for user: {msg.UserId}");
	}
}