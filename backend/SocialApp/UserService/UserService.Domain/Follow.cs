namespace UserService.Domain
{
	public class Follow
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		// The user who is following
		public Guid FollowerId { get; set; }
		public UserProfile Follower { get; set; } = null!;

		// The user being followed
		public Guid FollowingId { get; set; }
		public UserProfile Following { get; set; } = null!;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
