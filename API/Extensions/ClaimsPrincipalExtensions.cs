using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetMemberId(this ClaimsPrincipal user) {
        int id = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1");
        if(id == -1) {
            throw new Exception("there's a problem with the token");
        }

        return id;
    }
}
