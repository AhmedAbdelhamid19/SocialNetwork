using System;

namespace API.Helpers;

public class MessageParams
{
    public required int RecipientId { get; set; }
    public required string Content { get; set; }
}