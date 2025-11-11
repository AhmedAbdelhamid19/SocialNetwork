using System;

namespace API.Helpers;

public class SendMessageDTO
{
    public required int RecipientId { get; set; }
    public required string Content { get; set; }
}