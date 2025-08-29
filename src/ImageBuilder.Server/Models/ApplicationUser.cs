using Microsoft.AspNetCore.Identity;

namespace ImageBuilder.Server.Models;

public sealed class ApplicationUser : IdentityUser
{
    public bool IsApproved { get; set; }
}
