using App.Bal.Services;
using App.Entity;
using App.Entity.Dto;
using App.Entity.Http;
using App.Entity.Models.Auth;
using App.Entity.ViewModel;
using App.Foundation.Common;
using App.Foundation.Messages;
using App.Foundation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plivo.Exception;
using System.Security.Claims;

namespace Veribuild_latest.Controllers
{
    [Authorize]
    public class SettingController(
        IUserService userService, 
        IConfiguration configuration,
        IStorageService storageService) : Controller
    {

        private readonly IUserService _userService = userService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IStorageService _storageService = storageService;
        private readonly BaseVM _vm = new();


        #region Profile Manage

        public async Task<IActionResult> Index()
        {
            _vm.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _vm.User = await _userService.FindByIdWithCredentialAsync(_vm.UserId);
            _vm.Countries.AddRange(await _userService.GetCountries());
            _vm.BlobStorageUrl = _configuration.GetSection("AzureStorage:BlobStorageUrl").Value;
            return View(_vm);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumber(PhoneDto phoneDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!phoneDto.PhoneCode.StartsWith('+')) { phoneDto.PhoneCode = "+" + phoneDto.PhoneCode; }
                    AppUser? appUser = await _userService.FindByPhoneAsync(phoneDto.PhoneNumber, phoneDto.PhoneCode);
                    if (appUser != null)
                    {
                        return Json(new { code = 400, msg = ErrorMessages.UserExistWithPhone });
                    }
                    AppUser? existingUser = await _userService.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (existingUser != null)
                    {
                        Random random = new();
                        string otp = random.Next(111111, 999999).ToString();
                        bool isSent = MessageService.SendOtp(phoneDto.PhoneCode, phoneDto.PhoneNumber, otp);
                        if (isSent)
                        {
                            existingUser.OTP = otp;
                            existingUser.OTPCreateTime = DateTime.UtcNow;
                            await _userService.UpdateUserAsync(existingUser);
                            return Json(new AppResponse { Code = 200, Message = AppMessages.OTPSent });
                        }
                        return Json(new AppResponse { Code = 400, Message = ErrorMessages.Error500 });
                    }
                }
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new AppResponse { Code = 400, Message = string.Join("<br>", errors) });
            }
            catch (Exception e)
            {
                if (e is PlivoValidationException)
                {
                    return Json(new AppResponse { Code = 400, Message = ErrorMessages.PlivoMessageErrorMessage });
                }
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }


        [HttpPost]
        public async Task<IActionResult> VerifyPhoneVerifyOTP(PhoneDto phoneDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!phoneDto.PhoneCode.StartsWith('+')) { phoneDto.PhoneCode = "+" + phoneDto.PhoneCode; }
                    AppUser? existingUser = await _userService.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (existingUser != null)
                    {
                        string phoneNumber = (!phoneDto.PhoneCode.StartsWith('+') ? $"+{phoneDto.PhoneCode}" : phoneDto.PhoneCode) + phoneDto.PhoneNumber;
                        if (existingUser.OTPCreateTime != null && DateTime.UtcNow >= existingUser.OTPCreateTime.GetValueOrDefault().AddMinutes(Utils.OTPExpireMinutes))
                        {
                            return Json(new AppResponse { Code = 400, Message = ErrorMessages.OTPInvalid });
                        }
                        if (existingUser.OTP != phoneDto.Otp)
                        {
                            return Json(new AppResponse { Code = 400, Message = ErrorMessages.OTPInvalid });
                        }
                        existingUser.PhoneNumber = phoneDto.PhoneNumber;
                        existingUser.PhoneCode = phoneDto.PhoneCode;
                        existingUser.PhoneCodeId = phoneDto.PhoneCodeId;
                        await _userService.UpdateUserAsync(existingUser);
                        return Json(new AppResponse { Code = 200, Message = AppMessages.OTPVerifiedMessage });
                    }
                    return Json(new AppResponse { Code = 400, Message = ErrorMessages.Error500 });
                }
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                string errorMessage = string.Join("<br>", errors);
                return Json(new AppResponse { Code = 400, Message = errorMessage });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Update(ProfileDto profileDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!profileDto.CountryCode.StartsWith('+')) { profileDto.CountryCode = "+" + profileDto.CountryCode; }
                    AppUser? existingUser = await _userService.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (existingUser is not null)
                    {
                        string phoneWithCode = existingUser?.PhoneCode + existingUser?.PhoneNumber;
                        string phoneWithCodeUser = profileDto.CountryCode + profileDto.PhoneNumber;
                        if (!phoneWithCode.StartsWith('+'))
                        {
                            phoneWithCode = "+" + phoneWithCode;
                        }
                        if (!phoneWithCodeUser.StartsWith('+'))
                        {
                            phoneWithCodeUser = "+" + phoneWithCodeUser;
                        }
                        if (phoneWithCode != phoneWithCodeUser)
                        {
                            return Json(new AppResponse { Code = 400, Message = ErrorMessages.PhoneNotVerified });
                        }
                        existingUser!.PhoneCodeId = profileDto.CountryId;

                        existingUser!.FirstName = profileDto.FirstName;
                        existingUser.LastName = profileDto.LastName;
                        existingUser.AgencyName = profileDto.AgencyName;
                        existingUser.Address = profileDto.Address;
                        existingUser.Website = profileDto.Website;
                        //existingUser.Latlong = new NetTopologySuite.Geometries.Point(profileDto.Lattitude, profileDto.Lattitude) { SRID = 4326 };
                        if (profileDto.ProfileImage != null)
                        {
                            if (!string.IsNullOrEmpty(existingUser.Profile))
                            {
                                await _storageService.DeleteFile(existingUser.Profile);
                            }
                            string fileName = Guid.NewGuid().ToString() + DateTime.Now.ToString() + Path.GetExtension(profileDto.ProfileImage.FileName);
                            existingUser.Profile = "ProfileImages/" + fileName;
                            BlobResult result = await _storageService.UploadFile(file: profileDto.ProfileImage, fileName: fileName, folderName: "ProfileImages/");
                        }
                        await _userService.UpdateUserAsync(existingUser);
                        return Json(new AppResponse { Code = 200, Message = AppMessages.ProfileUpdateMessage });
                    }

                    return Json(new AppResponse { Code = 400, Message = ErrorMessages.Error500 });
                }
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                string errorMessage = string.Join("<br>", errors);
                return Json(new AppResponse { Code = 400, Message = errorMessage });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordDto passwordDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser? existingUser = await _userService.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    PasswordVerificationResult result = await _userService.ResetPassword(existingUser!, passwordDto);
                    if (result == PasswordVerificationResult.Success)
                        return Json(new AppResponse { Code = 200, Message = AppMessages.PasswordUpdateMessage });
                    return Json(new AppResponse { Code = 400, Message = ErrorMessages.InvalidCurrentPassword });
                }
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                string errorMessage = string.Join("<br>", errors);
                return Json(new AppResponse { Code = 400, Message = errorMessage });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }

        #endregion


        #region Credential Manage
        [HttpPost]
        public async Task<IActionResult> SaveCredentials()
        {
            try
            {
                AppUser? existingUser = await _userService.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _userService.SaveCredentails(existingUser!, Request.Form);
                return Json(new AppResponse { Code = 200, Message = AppMessages.CredentailSaveMessage });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCredential(long id)
        {
            try
            {
                AppResult result = await _userService.RemoveCredentails(id);
                return Json(result);
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }
        #endregion


        public IActionResult AccountDetails()
        {
            return View();
        }
    }
}
