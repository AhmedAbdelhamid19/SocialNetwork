using System;

namespace API.Helpers;

public class MessageParams: PagingParams
{
    public string Container { get; set; } = "Inbox";
}