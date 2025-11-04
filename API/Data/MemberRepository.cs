using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await context.Members
            .Include(m => m.Photos)
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member?> GetMemberForUpdate(int id)
    {
        return await context.Members.Include(m => m.User).Include(m => m.Photos).SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PaginatedResult<Member>> GetMembersAsync(PagingParams pagingParams)
    {
        var query = context.Members.AsQueryable();
        return await PaginationHelper.CreateAsync(query, pagingParams.PageNumber, pagingParams.PageSize);
     }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId)
    {
        return await context.Members.Where(x => x.Id == memberId).SelectMany(x => x.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
    }
}
