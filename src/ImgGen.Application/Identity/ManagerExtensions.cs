// Copyright (c) SC Strategic Solutions. All rights reserved.

using System.Text;
using Domain.Identity;
using ImgGen.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace ImgGen.Application.Identity;

public static class ManagerExtensions
{
    public static async Task<string> GenerateSecretAsync(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user)
    {
        if (user == null)
        {
            throw new Exception("User cannot be null");
        }

        var secret = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(secret));
    }


    public static async Task<ApplicationUser> GetRequiredUserWithId(this UserManager<ApplicationUser> self, Guid id)
    {
        return await self.GetUserWithId(id) ?? throw new Exception($"User with Id='{id.ToString()}' not found.");
    }

    public static Task<ApplicationUser?> GetUserWithId(this UserManager<ApplicationUser> self, Guid id)
    {
        return self.FindByIdAsync(id.ToString());
    }
}
