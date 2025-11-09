using System;

namespace API.Helpers;

public class FollowParams: PagingParams
{
    public string Predicate { get; set; } = "following";
}