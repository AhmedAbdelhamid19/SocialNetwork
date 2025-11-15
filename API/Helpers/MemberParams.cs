using System;

namespace API.Helpers;

public class MemberParams: PagingParams
{
    public string? Gender { get; set; } // if null then return male and females
    public int? CurrentMemberId { get; set; }
    public int MinAge { get; set; } = 18; // the default in app business logic
    public int MaxAge { get; set; } = 150; // the default in app business logic
    public string OrderBy { get; set; } = "lastActive"; // the default in app business logic
}