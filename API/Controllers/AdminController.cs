using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(UserManager<AppUser> userManager) : ControllerBase
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users.ToListAsync();
            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    user.Id,
                    user.UserName,
                    Roles = roles
                });
            }

            return Ok(usersWithRoles);
        }
        
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public IActionResult GetPhotosForModeration()
        {
            // Logic to retrieve users with their roles would go here.
            return Ok(new { Message = "This is a protected endpoint for admin, moderator users." });
        }


    }
}
