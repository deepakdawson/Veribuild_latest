using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace App.Entity.Models.Auth
{
    public class AppUserRole : IdentityRole
    {
        public virtual ICollection<AppUserRoleMapping> RoleMappings { get; set; } = null!;


        [StringLength(450)]
        public string? CreateBy { get;set; }
        public DateTime? CreateAt { get;set; }
    }
}
