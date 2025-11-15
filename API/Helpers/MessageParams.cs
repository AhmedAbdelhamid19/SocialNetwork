using System;

namespace API.Helpers;

public class MessageParams: PagingParams
{
    // inbox means message received and outbox means messages sent
    public string Container { get; set; } = "Inbox";
}