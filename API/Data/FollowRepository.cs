using System;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class FollowRepository(AppDbContext context) : IFollowRepository
{
    public async Task<MemberFollow?> GetFollowAsync(int sourceUserId, int targetUserId)
    {
        return await context.Follows
            .FirstOrDefaultAsync(f => f.SourceMemberId == sourceUserId && f.TargetMemberId == targetUserId);
    }

    public async Task<PaginatedResult<int>> GetAllFollowsIdsAsync(int userId, FollowParams followParams)
    {
        IQueryable<int> query = followParams.Predicate switch
        {
            "following" => context.Follows
                .Where(f => f.SourceMemberId == userId)
                .Select(f => f.TargetMemberId),
            "followers" => context.Follows
                .Where(f => f.TargetMemberId == userId)
                .Select(f => f.SourceMemberId),
            _ => throw new ArgumentException("Invalid predicate", nameof(followParams.Predicate))
        };
        
        return await PaginationHelper.CreateAsync(query, followParams.PageNumber, followParams.PageSize);
    }

    public async Task<PaginatedResult<Member>> GetAllFollowsAsync(int userId, FollowParams followParams)
    {
        IQueryable<Member> query = followParams.Predicate switch
        {
            "following" => context.Follows
                .Where(f => f.SourceMemberId == userId)
                .Select(f => f.TargetMember),
            "followers" => context.Follows
                .Where(f => f.TargetMemberId == userId)
                .Select(f => f.SourceMember),
            _ => throw new ArgumentException("Invalid predicate", nameof(followParams.Predicate))
        };
        
        return await PaginationHelper.CreateAsync(query, followParams.PageNumber, followParams.PageSize);
    }
    public void RemoveFollow(MemberFollow memberFollow)
    {
        context.Follows.Remove(memberFollow);
    }
    
    public void AddFollow(MemberFollow memberFollow)
    {
        context.Follows.AddAsync(memberFollow);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}