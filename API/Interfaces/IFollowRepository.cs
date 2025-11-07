using System;
using API.Entities;

namespace API.Interfaces;

public interface IFollowRepository
{
    // get a specific member follow relationship, e.g. to check if current member follows another member
    Task<MemberFollow?> GetMemberFollowAsync(int sourceUserId, int targetUserId);

    // get list of members that follow this member, or followed by this member
    Task<IReadOnlyList<Member>> GetMemberFollowsAsync(int userId, string predicate);

    // get list of ids that current member follows
    Task<IReadOnlyList<int>> GetCurrentMemberFolloweesAsync(int userId);

    // get list of ids that current member followed by them
    Task<IReadOnlyList<int>> GetCurrentMemberFollowersAsync(int userId);

    // member unfollow another member
    void RemoveMemberFollow(MemberFollow memberFollow);

    // member follow another member
    void AddMemberFollow(MemberFollow memberFollow);

    // save all changes to the database
    Task<bool> SaveAllAsync();
}
