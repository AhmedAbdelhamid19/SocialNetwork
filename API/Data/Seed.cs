using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Controllers;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {

        // check if any users in the database
        if (await context.Users.AnyAsync()) return;

        // read the json file
        var membersString = await File.ReadAllTextAsync("Data/UserSeedData.json");
        // deserialize the json file
        var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(membersString);

        if (members == null) { Console.WriteLine("No member in seed data"); return; }

        foreach (var member in members)
        {
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                Email = member.Email,
                DisplayName = member.DisplayName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
                Member = new Member
                {
                    DisplayName = member.DisplayName,
                    Description = member.Description,
                    DateOfBirth = member.DateOfBirth,
                    ImageUrl = member.ImageUrl,
                    Gender = member.Gender,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    LastActive = member.LastActive
                }
            };

            user.Member.Photos.Add(new Photo
            {
                Url = member.ImageUrl ?? string.Empty,
            });
            context.Users.Add(user);
        }
        await context.SaveChangesAsync();
    }
}
