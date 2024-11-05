using App.Entity.Dto.Custom;
using App.Entity.Models.Auth;
using App.Entity.Models.Property;

namespace App.Entity.ViewModel
{
    public class PropertyVM : BaseVM
    {
        public List<PropertyType> PropertyTypes { get; set; } = [];
        public List<Property> Properties { get; set; } = [];
        public List<PropertyContract> PropertyContracts { get; set; } = [];
        public List<PropertyDocument> PropertyDocuments { get; set; } = [];
        public List<UserInvite> UserInvites { get; set; } = [];
        public List<PropertyEventCDto> PropertyEvents { get; set; } = [];
        public List<EventType> Events { get; set; } = [];
        public int PropertyTypeId { get; set; }
        public Property? Property { get; set; }
        public PropertyContract? PropertyContract{ get; set; }
        public PropertyDocument? PropertyDocument{ get; set; }
        public AppUserRole? UserRole { get; set; }
        
    }
}
