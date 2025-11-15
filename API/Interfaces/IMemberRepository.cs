using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberByIdAsync(int id, bool includeUser = false, bool includePhotos = false);
    Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
    void Update(Member member);
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId);
    Task<bool> SaveAllAsync();

}
