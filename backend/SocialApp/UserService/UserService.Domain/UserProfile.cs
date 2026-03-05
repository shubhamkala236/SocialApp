namespace UserService.Domain;

public class UserProfile
{
	public Guid Id { get; set; } = Guid.NewGuid();

	// Synced from AuthService via JWT claims
	public Guid UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;

	public string? Bio { get; set; }
	public string? AvatarUrl { get; set; }
	public string? AvatarPublicId { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }

	// Navigation
	public ICollection<Follow> Followers { get; set; } = new List<Follow>();
	public ICollection<Follow> Following { get; set; } = new List<Follow>();
}