using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<Member?> GetMemberForUpdate(int id)
    {
        return await context.Members.Include(m => m.User).SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        // read only to avoid updated it (update only with Update)
        // read only to make it faster also
        return await context.Members.ToListAsync();
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId)
    {
        // select gives you list of lists, but selectMany flatten the result.
        // instead of [ [photos...] ] -> [ photos... ].
        return await context.Members.Where(x => x.Id == memberId).SelectMany(x => x.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        // Entry make the EF track the entity
        // change the state tell the EF to change it and update at save changes
        context.Entry(member).State = EntityState.Modified;
    }
}
