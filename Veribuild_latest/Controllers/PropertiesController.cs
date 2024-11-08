using App.Bal.Services;
using App.Entity.Dto;
using App.Entity.Http;
using App.Entity;
using App.Entity.ViewModel;
using App.Foundation.Common;
using App.Foundation.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Veribuild_latest.Controllers
{

    [Authorize]
    public class PropertiesController(
        IPropertyService propertyService,
        IConfiguration configuration) : Controller
    {

        private readonly IPropertyService _propertyService = propertyService;
        private readonly IConfiguration _configuration = configuration;
        private readonly PropertyVM _propertyVM = new ();

        public async Task<IActionResult> Index(string? address)
        {
            _propertyVM.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _propertyVM.Properties.AddRange(await _propertyService.GetProperties(_propertyVM.UserId, null, address));
            return View(_propertyVM);
        }
        

        public async Task<IActionResult> Add()
        {
            _propertyVM.PropertyTypes.AddRange(await _propertyService.GetPropertyTypesAsync());
            _propertyVM.GoogleApiKey = _configuration.GetSection("GoogleApiKey").Value!;
            return View(_propertyVM);  
        }


        [Authorize(Roles = UserRoles.RoleBuilder)]
        [HttpPost]
        public async Task<IActionResult> Add(PropertyDto propertyDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    propertyDto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    AppResult appResult = await _propertyService.AddProperty(propertyDto);
                    if (appResult.Success)
                    {
                        return Json(new AppResponse { Code = 200, Message = AppMessages.PropertyAddMessage });
                    }
                    return Json(new AppResponse { Code = 400, Message = appResult.Message! });
                }
                var errorList = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new AppResponse { Code = 400, Message = string.Join("</br>", errorList) });
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
                return Json(new AppResponse { Code = 500, Message = ErrorMessages.Error500 });
            }
        }
    }
}
