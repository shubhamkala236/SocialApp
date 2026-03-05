using System;
using System.Collections.Generic;
using System.Text;
using InteractionService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace InteractionService.Infrastructure.Context
{
	public class InteractionDbContext : DbContext
	{
		public InteractionDbContext(DbContextOptions<InteractionDbContext> options)
			: base(options) { }

		public DbSet<PostLike> PostLikes => Set<PostLike>();
		public DbSet<SavedPost> SavedPosts => Set<SavedPost>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// One like per user per post
			modelBuilder.Entity<PostLike>()
				.HasIndex(l => new { l.PostId, l.UserId })
				.IsUnique();

			// One save per user per post
			modelBuilder.Entity<SavedPost>()
				.HasIndex(s => new { s.PostId, s.UserId })
				.IsUnique();
		}
	}

}
