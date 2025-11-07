using System;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class FollowRepository(AppDbContext context) : IFollowRepository
{
    public async Task<MemberFollow?> GetMemberFollowAsync(int sourceUserId, int targetUserId)
    {
        return await context.Follows
            .FirstOrDefaultAsync(f => f.SourceMemberId == sourceUserId && f.TargetMemberId == targetUserId);
    }

    public async Task<IReadOnlyList<Member>> GetMemberFollowsAsync(int userId, string predicate)
    {
        return predicate switch
        {
            "followees" => await context.Follows
                .Where(f => f.SourceMemberId == userId)
                .Select(f => f.TargetMember)
                .ToListAsync(),

            "followers" => await context.Follows
                .Where(f => f.TargetMemberId == userId)
                .Select(f => f.SourceMember)
                .ToListAsync(),

            _ => throw new ArgumentException("Invalid predicate", nameof(predicate))
        };
    }

    public async Task<IReadOnlyList<int>> GetCurrentMemberFolloweesAsync(int userId)
    {
        return await context.Follows
            .Where(f => f.SourceMemberId == userId)
            .Select(f => f.TargetMemberId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<int>> GetCurrentMemberFollowersAsync(int userId)
    {
        return await context.Follows
            .Where(f => f.TargetMemberId == userId)
            .Select(f => f.SourceMemberId)
            .ToListAsync();
    }

    public void RemoveMemberFollow(MemberFollow memberFollow)
    {
        context.Follows.Remove(memberFollow);
    }
    
    public void AddMemberFollow(MemberFollow memberFollow)
    {
        context.Follows.AddAsync(memberFollow);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}