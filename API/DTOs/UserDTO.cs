using System;

namespace API.DTOs;

public class UserDTO
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string Token { get; set; }
    public string? ImageUrl { get; set; }
}
