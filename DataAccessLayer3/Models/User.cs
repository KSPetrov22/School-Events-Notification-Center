namespace DataAccessLayer3.Models;

public sealed class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<SchoolEvent> OrganizedEvents { get; set; } = [];
    public ICollection<Registration> Registrations { get; set; } = [];
}
