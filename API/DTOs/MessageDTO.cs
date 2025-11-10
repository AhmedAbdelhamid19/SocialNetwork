using System;

namespace API.DTOs;

public class MessageDTO
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
    public int SenderId { get; set; }
    public string SenderDisplayName { get; set; } = null!;
    public string? SenderImageUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientDisplayName { get; set; } = null!;
    public string? RecipientImageUrl { get; set; }
}
