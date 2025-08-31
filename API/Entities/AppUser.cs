using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string DisplayName { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string Email { get; set; }

    [Required]
    public required byte[] PasswordHash { get; set; }

    [Required]
    public required byte[] PasswordSalt { get; set; }

    public string? ImageUrl { get; set; }

}
