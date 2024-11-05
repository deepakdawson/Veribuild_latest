using App.Entity.Models;
using App.Entity.Models.Auth;
using App.Entity.ViewModel.Interfaces;

namespace App.Entity.ViewModel
{
    public class BaseVM : IBaseModel
    {
        public BaseVM()
        {
            Countries = [];
        }

        public List<Country> Countries { get; set; }
        public AppUser? User { get; set; }
        public string GoogleApiKey { get; set; } = string.Empty;
        public List<AppUserRole> UserRoles { get; set; } = [];

    }
}
