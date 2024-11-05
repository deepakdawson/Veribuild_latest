using App.Entity.Models;
using App.Entity.Models.Auth;
using App.Entity.Models.Property;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Dal
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<
        AppUser, AppUserRole, string,
        IdentityUserClaim<string>,
        AppUserRoleMapping,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>(options), IDataProtectionKeyContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("vb8");

            builder.Entity<AppUser>(b =>
            {
                // Maps to the AspNetUsers table
                b.ToTable("Users");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                // Maps to the AspNetUserClaims table
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                // Maps to the AspNetUserLogins table
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                // Maps to the AspNetUserTokens table
                b.ToTable("UserTokens");
            });

            builder.Entity<AppUserRole>(b =>
            {
                // Maps to the AspNetRoles table
                b.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                // Maps to the AspNetRoleClaims table
                b.ToTable("RoleClaims");
            });

            builder.Entity<AppUserRoleMapping>(b =>
            {
                // Maps to the AspNetUserRoles table
                b.ToTable("UserRoles");
            });

            builder.Entity<AppUserRoleMapping>(e =>
            {
                e.HasKey(b => new { b.UserId, b.RoleId });
                e.ToTable("UserRoles");
                e.HasOne(ur => ur.AppUserRole)
                .WithMany(r => r.RoleMappings)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

                e.HasOne(ur => ur.AppUser)
                    .WithMany(r => r.UserRoleMappings)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<PropertyType> PropertyTypes { get; set; } = null!;
        public DbSet<UserCredential> UserCredentials { get; set; } = null!;
        public DbSet<Property> Properties{ get; set; } = null!;
        public DbSet<PropertyFile> PropertyFiles { get; set; } = null!;
        public DbSet<PropertyContract> PropertyContracts { get; set; } = null!;
        public DbSet<PropertyDocument> PropertyDocuments { get; set; } = null!;
        public DbSet<PropertyEvent> PropertyEvents { get; set; } = null!;
        public DbSet<UserInvite> UserInvites { get; set; } = null!;
        public DbSet<EventType> EventTypes { get; set; } = null!;
    }
}
