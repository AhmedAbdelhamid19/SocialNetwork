using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string DisplayName { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Email { get; set; }
}
