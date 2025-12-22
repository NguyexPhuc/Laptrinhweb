using Microsoft.AspNetCore.Identity;

namespace Web_BanHang.Models
{
    // Minimal stub for ApplicationUser to allow design-time builds.
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}