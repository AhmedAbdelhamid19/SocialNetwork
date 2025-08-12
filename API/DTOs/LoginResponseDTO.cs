using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginResponseDTO
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Token { get; set; } = ""; // For future JWT implementation
} 