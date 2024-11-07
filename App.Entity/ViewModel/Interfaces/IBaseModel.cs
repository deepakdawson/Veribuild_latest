using App.Entity.Models;
using App.Entity.Models.Auth;

namespace App.Entity.ViewModel.Interfaces
{
    public interface IBaseModel
    {
        public List<Country> Countries { get; set; }
        public AppUser? User { get; set; }
        public string GoogleApiKey { get; set; }
        public List<AppUserRole> UserRoles { get; set; }
        public string? UserId {  get; set; }
        public string? BlobStorageUrl {  get; set; }
    }
}
