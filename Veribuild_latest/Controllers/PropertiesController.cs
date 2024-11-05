using App.Bal.Services;
using App.Entity.ViewModel;
using App.Foundation.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veribuild_latest.Controllers
{

    [Authorize(Roles = UserRoles.RoleBuilder)]
    public class PropertiesController(
        IPropertyService propertyService,
        IConfiguration configuration) : Controller
    {

        private readonly IPropertyService _propertyService = propertyService;
        private readonly IConfiguration _configuration = configuration;
        private readonly PropertyVM _propertyVM = new ();

        public IActionResult Index()
        {
            return View();
        }
        

        public async Task<IActionResult> Add()
        {
            _propertyVM.PropertyTypes.AddRange(await _propertyService.GetPropertyTypesAsync());
            _propertyVM.GoogleApiKey = _configuration.GetSection("GoogleApiKey").Value!;
            return View(_propertyVM);  
        }

    }
}
