using API.Entities;

namespace API.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetMemberByIdAsync(int id);
    Task<IReadOnlyList<Member>> GetMembersAsync();
    void Update(Member member);
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId);
}
