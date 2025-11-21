using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(int id, bool includeUser = true, bool includePhotos = true)
    {
        var query = context.Members.AsQueryable();
        
        if (includeUser)
            query = query.Include(m => m.User);
        
        if (includePhotos)
            query = query.Include(m => m.Photos);
        
        return await query.FirstOrDefaultAsync(m => m.Id == id);
    } 
    public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
    {
        var query = context.Members.AsQueryable();
        query = query.Where(m => m.Id != memberParams.CurrentMemberId);
        if (memberParams.Gender != null)
        {
            query = query.Where(m => m.Gender == memberParams.Gender);
        }
        query = memberParams.OrderBy switch
        {
            "created" => query.OrderByDescending(m => m.Created),
            _ => query.OrderByDescending(m => m.LastActive)
        };
        var minDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
        var maxDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

        query = query.Where(m => m.DateOfBirth >= minDateOfBirth && m.DateOfBirth <= maxDateOfBirth);
        return await PaginationHelper.CreateAsync(query, memberParams.PageNumber, memberParams.PageSize);
    }
    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(int memberId)
    {
        return await context.Members
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Photos).ToListAsync();
    }
    public void Update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
    }
    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}