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
            return Ok(new { Message = "admin, moderator only." });
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{id}")]
         public async Task<IActionResult> EditRoles(string id, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Could not find user");

            var userRoles = await userManager.GetRolesAsync(user);

            // for example, if user is in role "Member" and selectedRoles contains "Member",
            // we don't need to add "Member" again, so we use Except to get only the roles 
            // that are not already assigned to the user.
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            // for example, if user is in role "Admin" but selectedRoles does not contain "Admin",
            // we need to remove "Admin" from the user, so we use Except to get only the roles that 
            // are assigned to the user but not in selectedRoles.
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }
    }
}
