using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register) 
        {
            if(await EmailExist(register.Email))
                return BadRequest("Email taken before");

            using var hmac = new HMACSHA512();

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

            context.Users.Add(user);

            if(await context.SaveChangesAsync() > 0)
            {
                return Ok(user.ToDto(tokenService));
            }
            return BadRequest("Problem registering user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login) 
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email!.Equals(login.Email));
            if(user == null)
                return Unauthorized("Invalid email address");
                
             
            
            return user.ToDto(tokenService);
        }

        private async Task<bool> EmailExist(string email) 
        {
            return await context.Users.AnyAsync(u => u.Email!.ToLower().Equals(email.ToLower()));
        }
    }
}
