using App.Bal.Services;
using App.Dal;
using App.Entity;
using App.Entity.Config;
using App.Entity.Dto;
using App.Entity.Http;
using App.Entity.Models;
using App.Entity.Models.Auth;
using App.Entity.Models.Property;
using App.Foundation.Common;
using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Plivo.Resource;

namespace App.Bal.Repositories
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppUserRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly AppConfig _appConfig;

        public UserService(AppDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppUserRole> roleManager,
            SignInManager<AppUser> signInManager,
            IPasswordHasher<AppUser> passwordHasher,
            IStorageService storageService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = dbContext;
            _passwordHasher = passwordHasher;
            _storageService = storageService;
            _configuration = configuration;
            _appConfig = new();
            _configuration.GetSection(AppConfig.Key).Bind(_appConfig);
        }

        #region User Access Methods
        public async Task<bool> CreateUserAsync(AppUser appUser)
        {
            return (await _userManager.CreateAsync(appUser)).Succeeded;
        }

        public async Task<AppUser?> FindByEmailAsync(string? email)
        {
            return await _userManager.FindByEmailAsync(email!);
        }

        public async Task<AppUser?> FindByIdAsync(string? id)
        {
            return await _userManager.FindByIdAsync(id!);
        }

        public async Task<AppUser?> FindByIdWithCredentialAsync(string? id)
        {
            return await _context.Users.Include(e => e.UserCredentials).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<AppUser?> FindByPhoneAsync(string phoneNumber, string phoneCode)
        {
            if (!phoneCode.StartsWith('+'))
            {
                phoneCode = "+" + phoneCode;
            }
            return await _context.Users.FirstOrDefaultAsync(e => e.PhoneNumber == phoneNumber && e.PhoneCode == phoneCode);
        }

        public async Task<AppUser?> FindByEmailPhoneAsync(string email, string? phoneNumber, string? phoneCode)
        {
            AppUser? appUser = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            if (appUser is null)
            {
                if (phoneCode?.StartsWith('+') == false)
                {
                    phoneCode = "+" + phoneCode;
                }
                appUser = await _context.Users.FirstOrDefaultAsync(e => e.PhoneCode == phoneCode && e.PhoneNumber == phoneNumber);
            }
            return appUser;
        }

        #endregion


        #region Authentication Methods
        public async Task SignInOtpAsync(AppUser appUser)
        {
            await _signInManager.SignInAsync(appUser, true);
        }

        public async Task<bool> SignInEmailAsync(AppUser appUser, string password, bool isPersistent, bool lockoutOnFailure)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(appUser, password, isPersistent, lockoutOnFailure);
            return result.Succeeded;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task UpdateUserAsync(AppUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<PasswordVerificationResult> ResetPassword(AppUser user, PasswordDto dto)
        {
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Success)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                return PasswordVerificationResult.Success;
            }
            return PasswordVerificationResult.Failed;
        }

        public async Task<PasswordVerificationResult> ResetPassword(AppUser user, string password)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, password);
            return result.Succeeded ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
        #endregion


        #region Role Management
        public async Task<bool> AddRolesAsync(AppUser appUser, IEnumerable<string> roles)
        {
            IdentityResult result = await _userManager.AddToRolesAsync(appUser, roles);
            return result.Succeeded;
        }

        public Task<List<AppUserRole>> GetRolesAsync()
        {
            return _roleManager.Roles.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<List<AppUserRole>> GetRolesExceptBuilderAsync()
        {
            return await _context.Roles.Where(e => e.Name != UserRoles.RoleBuilder).OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRolesAsync(AppUser appUser)
        {
            return await _userManager.GetRolesAsync(appUser);
        }

        public async Task<bool> RemoveRolesAsync(AppUser appUser, IEnumerable<string> roles)
        {
            IdentityResult result = await _userManager.RemoveFromRolesAsync(appUser, roles);
            return result.Succeeded;
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            IdentityResult result = await _roleManager.CreateAsync(new AppUserRole() { Name = roleName });
            return result.Succeeded;
        }

        public async Task<AppResult> CreateCustomRoleAsync(string roleName, string userId)
        {
            AppUserRole? role = await _roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                return new AppResult() { Success = false, Message = ErrorMessages.RoleExist };
            }
            AppUserRole appUserRole = new()
            {
                Name = roleName,
                CreateBy = userId,
                CreateAt = DateTime.UtcNow
            };
            IdentityResult result = await _roleManager.CreateAsync(appUserRole);
            if (result.Succeeded)
            {
                return new AppResult() { Success = true };
            }
            return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
        }
        #endregion


        #region Other Methods
        public async Task<List<Country>> GetCountries()
        {
            return await _context.Countries.OrderBy(e => e.NickName).ToListAsync();
        }
        #endregion


        #region Profile Methods
        public async Task<bool> SaveCredentails(AppUser appUser, IFormCollection collection)
        {
            List<UserCredential> userCredentials = [];
            int totalFiles = Convert.ToInt32(collection["TotalFiles"]);
            for (int i = 1; i <= totalFiles; i++)
            {
                IFormFile? file = collection.Files.GetFile($"file{i}");
                if (file is not null)
                {
                    string fileName = $"{Guid.NewGuid()}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
                    BlobResult blobResult = await _storageService.UploadFile(file, fileName, _appConfig.UserCredential);
                    userCredentials.Add(new UserCredential()
                    {
                        UserId = appUser.Id,
                        Name = collection[$"fileTitle{i}"],
                        BlobName = blobResult.BlobName,
                        Path = blobResult.Uri
                    });
                }
            }
            await _context.UserCredentials.AddRangeAsync(userCredentials);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AppResult> RemoveCredentails(long id)
        {
            UserCredential? credential = await _context.UserCredentials.FindAsync(id);
            if (credential is not null)
            {
                if (await _storageService.DeleteFile(credential.BlobName!)) 
                {
                    _context.UserCredentials.Remove(credential);
                    await _context.SaveChangesAsync();
                    return new AppResult() { Code = 200, Message = AppMessages.CredentailRemoveMessage };
                }
            }
            return new AppResult() { Code = 400, Message = ErrorMessages.InvlaidCredential };
        }

        #endregion


    }
}
