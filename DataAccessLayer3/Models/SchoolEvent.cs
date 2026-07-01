namespace DataAccessLayer3.Models;

public sealed class SchoolEvent
{
    public string Id { get; set; } = string.Empty;
    public string OrganizerId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StartsAt { get; set; } = string.Empty;
    public string EndsAt { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; } = "DRAFT";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? Organizer { get; set; }
    public ICollection<Registration> Registrations { get; set; } = [];
}
