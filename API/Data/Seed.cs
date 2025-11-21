using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Controllers;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        // check if any users in the database
        if (await userManager.Users.AnyAsync()) return;

        // read the json file as nullable string
        var membersString = await File.ReadAllTextAsync("Data/UserSeedData.json");
        // deserialize the json file
        var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(membersString);

        if (members == null) { Console.WriteLine("No member in seed data"); return; }

        foreach (var member in members)
        {
            var user = new AppUser
            {
                Email = member.Email,
                DisplayName = member.DisplayName,
                UserName = member.Email,
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
                },
                ImageUrl = member.ImageUrl
            };

            user.Member.Photos.Add(new Photo
            {
                Url = member.ImageUrl ?? string.Empty,
            });
            var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if(!result.Succeeded)
            {
                throw new Exception("Failed to create seed user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            await userManager.AddToRoleAsync(user, "Member");
        }
        var admin = new AppUser
        {
            DisplayName = "Admin",
            Email = "admin@test.com",
            UserName = "admin@test.com"
        };
        var adminResult = await userManager.CreateAsync(admin, "Pa$$w0rd");
        if(!adminResult.Succeeded)
        {
            throw new Exception("Failed to create admin user: " + 
                string.Join(", ", adminResult.Errors.Select(e => e.Description)));
        }
        await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"] );
    }
}
