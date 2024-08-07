﻿using System.Security.Claims;

public class UserModel
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public static UserModel GetUserInfoFromClaims(ClaimsPrincipal user)
    {
        var userModel = new UserModel();

        if (user.Identity.IsAuthenticated)
        {
            var claims = user.Claims;

            var nameClaim = user.FindFirstValue(ClaimTypes.Name);
            if (nameClaim != null) { userModel.Name = nameClaim; }

            var roleClaim = user.FindFirstValue(ClaimTypes.Role);
            if (roleClaim != null) { userModel.Role = roleClaim; }
        }

        return userModel;
    }
}