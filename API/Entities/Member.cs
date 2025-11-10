using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public int Id { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string DisplayName { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }

    [JsonIgnore]
    public ICollection<Photo> Photos { get; set; } = [];
    [JsonIgnore]
    public List<MemberFollow> Following { get; set; } = [];
    [JsonIgnore]
    public List<MemberFollow> Followers { get; set; } = [];
    [JsonIgnore]
    public List<Message> MessagesSent { get; set; } = [];
    [JsonIgnore]
    public List<Message> MessagesRecieved { get; set; } = [];

    [JsonIgnore]
    // Navigation Property
    [ForeignKey(nameof(Id))]
    public AppUser User { get; set; } = null!;
}
