using System;

namespace API.Data;

public class Connection(string connectionId, int userId)
{
    public string ConnectionId { get; set; } = connectionId;
    public int UserId { get; set; } = userId;

    public Group Group { get; set; } = null!;
}