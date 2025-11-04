using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberByIdAsync(int id);
    Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
    void Update(Member member);
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId);
    Task<Member?> GetMemberForUpdate(int id);
}
