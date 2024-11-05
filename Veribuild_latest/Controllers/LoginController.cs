using App.Bal.Services;
using App.Entity.Dto;
using App.Entity.Http;
using App.Entity.Models.Auth;
using App.Entity.ViewModel;
using App.Entity.ViewModel.Interfaces;
using App.Foundation.Common;
using App.Foundation.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Veribuild_latest.Controllers
{
    public class LoginController(IUserService userService) : Controller
    {

        private readonly IUserService _userService = userService;
        private readonly IBaseModel viewModel = new BaseVM();

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("add", "properties");
            }

            viewModel.Countries.AddRange(await _userService.GetCountries());
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ValidateSignin(SignInDto signInDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AppUser? appUser = null;
                    if (signInDto.IsEmailLogin)
                    {
                        appUser = await _userService.FindByEmailAsync(signInDto.Email);
                    }
                    else
                    {
                        appUser = await _userService.FindByEmailPhoneAsync("", signInDto.PhoneNumber, signInDto.PhoneCode);
                    }
                    if (appUser == null)
                    {
                        return Json(new AppResponse() { Code = 400, Message = ErrorMessages.InvlaidCredential });
                    }
                    if (!appUser.IsActive)
                    {
                        return Json(new AppResponse() { Code = 400, Message = ErrorMessages.AccountLocked });
                    }
                    // clear any previous
                    await _userService.SignOutAsync();
                    bool isSignIn = await _userService.SignInEmailAsync(appUser, signInDto.Password, isPersistent: true, lockoutOnFailure: false);
                    if (isSignIn)
                    {
                        return Json(new AppResponse() { Code = 200, Message = Url.Action("add", "properties")! });
                    }
                    return Json(new AppResponse() { Code = 400, Message = ErrorMessages.InvlaidCredential });
                }
                List<string> errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new AppResponse() { Code = 400, Message = string.Join("<br>", errors) });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse() { Code = 500, Message = ErrorMessages.Error500 });
            }
        }

    }
}
