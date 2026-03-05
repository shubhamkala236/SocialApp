using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService.Infrastructure.Context
{
	public class UserDbContext : DbContext
	{
		public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

		public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
		public DbSet<Follow> Follows => Set<Follow>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Unique constraint — can't follow same person twice
			modelBuilder.Entity<Follow>()
				.HasIndex(f => new { f.FollowerId, f.FollowingId })
				.IsUnique();

			// Can't follow yourself (check constraint)
			modelBuilder.Entity<Follow>()
				.ToTable(t => t.HasCheckConstraint(
					"CK_Follow_NoSelfFollow",
					"FollowerId <> FollowingId"));

			// Follower relationship
			modelBuilder.Entity<Follow>()
				.HasOne(f => f.Follower)
				.WithMany(u => u.Following)
				.HasForeignKey(f => f.FollowerId)
				.OnDelete(DeleteBehavior.Restrict);

			// Following relationship
			modelBuilder.Entity<Follow>()
				.HasOne(f => f.Following)
				.WithMany(u => u.Followers)
				.HasForeignKey(f => f.FollowingId)
				.OnDelete(DeleteBehavior.Restrict);

			// Unique userId per profile
			modelBuilder.Entity<UserProfile>()
				.HasIndex(u => u.UserId)
				.IsUnique();
		}
	}
}
