using Microsoft.EntityFrameworkCore;
using PostService.Domain;

namespace PostService.Infrastructure.Context
{
	public class PostDbContext : DbContext
	{
		public PostDbContext(DbContextOptions<PostDbContext> options) : base(options) { }

		public DbSet<Post> Posts => Set<Post>();
	}
}
