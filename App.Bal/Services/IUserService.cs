using App.Entity;
using App.Entity.Dto;
using App.Entity.Models;
using App.Entity.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace App.Bal.Services
{
    public interface IUserService
    {
        #region User Access Methods
        public Task<bool> CreateUserAsync(AppUser appUser);
        public Task<AppUser?> FindByEmailAsync(string? email);
        public Task<AppUser?> FindByIdAsync(string? id);
        public Task<AppUser?> FindByIdWithCredentialAsync(string? id);
        public Task<AppUser?> FindByPhoneAsync(string phoneNumber, string phoneCode);
        public Task<AppUser?> FindByEmailPhoneAsync(string email, string? phoneNumber, string? phoneCode);
        #endregion

        #region Authentication Methods
        public Task SignInOtpAsync(AppUser appUser);
        public Task<bool> SignInEmailAsync(AppUser appUser, string password, bool isPersistent, bool lockoutOnFailure);
        public Task SignOutAsync();
        public Task UpdateUserAsync(AppUser user);
        public Task<PasswordVerificationResult> ResetPassword(AppUser user, PasswordDto dto);
        public Task<PasswordVerificationResult> ResetPassword(AppUser user, string password);

        #endregion

        #region Role Management
        public Task<List<AppUserRole>> GetRolesAsync();
        public Task<List<AppUserRole>> GetRolesExceptBuilderAsync();
        public Task<bool> CreateRoleAsync(string roleName);
        public Task<IEnumerable<string>> GetRolesAsync(AppUser appUser);
        public Task<bool> RemoveRolesAsync(AppUser appUser, IEnumerable<string> roles);
        public Task<bool> AddRolesAsync(AppUser appUser, IEnumerable<string> roles);
        public Task<AppResult> CreateCustomRoleAsync(string roleName, string userId);
        #endregion

        #region Profile Methods
        public Task<bool> SaveCredentails(AppUser appUser, IFormCollection collection);
        public Task<bool> RemoveCredentails(long id);
        #endregion

        #region Other Methods
        Task<List<Country>> GetCountries();
        #endregion
    }
}
