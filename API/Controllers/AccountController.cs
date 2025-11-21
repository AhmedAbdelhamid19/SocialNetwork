using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController 
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register) 
        {
            var user = new AppUser {
                DisplayName = register.DisplayName,
                Email = register.Email,
                UserName = register.Email,
                Member = new Member
                {
                    DisplayName = register.DisplayName,
                    Gender = register.Gender,
                    City = register.City,
                    Country = register.Country,
                    DateOfBirth = register.DateOfBirth
                }
            };

            var result = userManager.CreateAsync(user, register.Password);
            if(!result.Result.Succeeded)
            {
                foreach(var error in result.Result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }
                return ValidationProblem();
            }
            await userManager.AddToRoleAsync(user, "Member");
            return await user.ToDto(tokenService);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login) 
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if(user == null)
                return Unauthorized("email or password is invalid");
                
            var result = await userManager.CheckPasswordAsync(user, login.Password);
            if(!result)
                return Unauthorized("email or password is invalid");
            return await user.ToDto(tokenService);
        }
    }
}
