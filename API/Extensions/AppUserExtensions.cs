using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;

namespace API.Extensions;

public static class AppUserExtensions
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static async Task<UserDTO> ToDto(this AppUser user, ITokenService tokenService) 
    {
        return new UserDTO {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email!,
            Token = await tokenService.CreateToken(user),
            ImageUrl = user.ImageUrl
        };
    }
}
