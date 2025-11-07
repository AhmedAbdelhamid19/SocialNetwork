using System;
using API.Entities;

namespace API.Interfaces;

public interface IFollowRepository
{
    /// <summary>
    /// get a specific member follow relationship, e.g. to check if specific member follows another member
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <returns></returns>
    Task<MemberFollow?> GetFollowAsync(int sourceUserId, int targetUserId);

    /// <summary>
    /// get list of members' ids that follow this member, or followed by this member
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<IReadOnlyList<int>> GetAllFollowsIdsAsync(int userId, string predicate);

    Task<IReadOnlyList<Member>> GetAllFollowsAsync(int userId, string predicate);

    /// <summary>
    /// member unfollow another member
    /// </summary>
    /// <param name="memberFollow"></param>
    void RemoveFollow(MemberFollow memberFollow);

    /// <summary>
    /// member follow another member
    /// </summary>
    /// <param name="memberFollow"></param>
    void AddFollow(MemberFollow memberFollow);

    /// <summary>
    /// save all changes to the database
    /// </summary>
    /// <returns></returns>
    Task<bool> SaveAllAsync();
}