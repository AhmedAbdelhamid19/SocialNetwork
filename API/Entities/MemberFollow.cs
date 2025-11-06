using System;

namespace API.Entities;

public class MemberFollow
{
    public int SourceMemberId { get; set; }
    public Member SourceMember { get; set; } = null!;
    public int TargetMemberId { get; set; }
    public Member TargetMember { get; set; } = null!;
}