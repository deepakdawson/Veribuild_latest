using App.Entity;
using App.Entity.Dto;
using App.Entity.Dto.Custom;
using App.Entity.Http;
using App.Entity.Models.Property;

namespace App.Bal.Services
{
    public interface IPropertyService
    {
        public Task<int> FindPropertyMatch(string address);
        public Task<List<PropertyType>> GetPropertyTypesAsync();
        public Task<AppResult> AddProperty(PropertyDto propertyDto);
        public Task<List<Property>> GetProperties(string userId);
        public Task<List<Property>> GetProperties(string userId, int propertyTypeId, string address);
        public Task<Property?> FindProperty(long id);
        public Task<Property?> FindProperty(string uniqueId);
        public Task<Property?> FindPropertyAlon(string uniqueId);

        public Task<List<PropertyEventCDto>> GetPropertyEvents(long id, string currentUserRole);
        public Task<List<PropertyEventCDto>> GetPropertyEventsAll(long id);


        #region Property Contract
        public Task<List<PropertyContract>> FindPropertyContracts(long propertyId);
        public Task<PropertyContract?> FindPropertyContracts(string uniqueId);
        public Task<PropertyContract?> FindPropertyActiveContracts(long propertyId);
        public Task<AppResult> AddContract(ContractDto contractDto);
        public Task<bool> RemoveContract(long id);
        #endregion


        #region Property Doucment
        public Task<List<PropertyDocument>> FindPropertyDocument(long propertyId);
        public Task<PropertyDocument?> FindPropertyDocument(string uniqueId);
        public Task<AppResult> AddDocument(DocumentDto documentDto);
        public Task<bool> RemoveDocument(long id);
        #endregion


        #region Invite
        public Task<List<UserInvite>> FindUserInvite(long propertyId);
        public Task<List<UserInviteCustomDto>> FindUserInviteByContract(long contractId);
        public Task<AppResult> InviteUser(UserInviteDto dto);
        #endregion


        #region Blockchain
        public Task<QrApiResult?> GenerateQrCode();
        #endregion


        #region Events management
        public Task<bool> AddEvent(string eventName, string createdByUserId);
        public Task<AppResult> SaveCustomEvent(ManagePropertyDto dto);
        public Task<EventType?> FindEventAsync(string eventName);
        public Task<EventType?> FindEventAsync(int eventId);
        public Task<List<EventType>> GetEventsAsync();
        #endregion
    }
}
