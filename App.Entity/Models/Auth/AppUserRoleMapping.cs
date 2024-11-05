using Microsoft.AspNetCore.Identity;

namespace App.Entity.Models.Auth
{
    public class AppUserRoleMapping : IdentityUserRole<string>
    {
        public virtual AppUser AppUser { get; set; } = null!;
        public virtual AppUserRole AppUserRole { get; set; } = null!;
    }
}
