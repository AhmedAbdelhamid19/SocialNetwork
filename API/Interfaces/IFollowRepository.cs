using System;
using API.Entities;

namespace API.Interfaces;

public interface IFollowRepository
{
    // get a specific member follow relationship, e.g. to check if specific member follows another member
    Task<MemberFollow?> GetFollowAsync(int sourceUserId, int targetUserId);

    // get list of members that follow this member, or followed by this member
    Task<IReadOnlyList<int>> GetAllFollowsIdsAsync(int userId, string predicate);

    // get list of ids that current member follows
    Task<IReadOnlyList<int>> GetMemberFolloweesAsync(int userId);

    // get list of ids that current member followed by them
    Task<IReadOnlyList<int>> GetMemberFollowersAsync(int userId);

    // member unfollow another member
    void RemoveFollow(MemberFollow memberFollow);

    // member follow another member
    void AddFollow(MemberFollow memberFollow);

    // save all changes to the database
    Task<bool> SaveAllAsync();
}
