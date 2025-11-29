
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

            await SetRefreshToken(user);
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

            await SetRefreshToken(user);
            return await user.ToDto(tokenService);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDTO>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(refreshToken == null)  return NoContent(); 

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                    u.RefreshTokenExpiryTime > DateTime.UtcNow);
            if(user == null) return Unauthorized("Invalid Refresh Token");

            // set new refresh token in user table and in cookie
            await SetRefreshToken(user);
            // return new jwt access token for 7 minutes
            return await user.ToDto(tokenService);
        }
        
        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = tokenService.CreateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // not accessible via JavaScript
                Expires = user.RefreshTokenExpiryTime,
                SameSite = SameSiteMode.Strict, // mitigate CSRF attacks
                Secure = true // ensure the cookie is only sent over HTTPS
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout() 
        {
            await userManager.Users
                .Where(u => u.Id == User.GetMemberId())
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.RefreshToken, _ => null)
                    .SetProperty(x => x.RefreshTokenExpiryTime, _ => DateTime.MinValue)
                );
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }
    }
} 