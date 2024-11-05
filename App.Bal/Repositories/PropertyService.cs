using App.Bal.Services;
using App.Bal.Support;
using App.Dal;
using App.Entity;
using App.Entity.Config;
using App.Entity.Dto;
using App.Entity.Dto.Custom;
using App.Entity.Http;
using App.Entity.Models.Auth;
using App.Entity.Models.Property;
using App.Foundation.Common;
using App.Foundation.Enumeration;
using App.Foundation.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace App.Bal.Repositories
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _dbContext;
        private readonly IBlockchainService _blockchainService;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailService _mailService;
        private readonly AppConfig _config;

        public PropertyService(AppDbContext appDbContext,
        IBlockchainService blockchainService,
        IStorageService storageService, 
        IConfiguration configuration,
        UserManager<AppUser> userManager,
        IMailService mailService)
        {
            _dbContext = appDbContext;
            _blockchainService = blockchainService;
            _storageService = storageService;
            _configuration = configuration;
            _userManager = userManager;
            _mailService = mailService;
            _config = new();
            _configuration.GetSection(AppConfig.Key).Bind(_config);
        }

        #region Manage Property
        public async Task<AppResult> AddProperty(PropertyDto propertyDto)
        {
            List<PropertyImage> propertyImages = [];
            PropertyType? propertyType = await _dbContext.PropertyTypes.FindAsync(propertyDto.PropertyTypeId);
            // upload floor plan pdf
            string fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + Path.GetExtension(propertyDto.FloorPlan?.FileName); 
            BlobResult blobResult = await _storageService.UploadFile(propertyDto.FloorPlan!, fileName, _config.FloorPlanDoc);
            if (string.IsNullOrEmpty(blobResult.Uri))
            {
                LoggerHelper.LogError(new Exception(ErrorMessages.BlobUploadError));
                return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
            }
            string metaData = $"Address: {propertyDto.Address}||Lat: {propertyDto.Lattitude}||Long: {propertyDto.Longitude}||Area: {propertyDto.Area}M2||Type: {propertyType?.Name}||Bedroom: {propertyDto.Bedroom}";

            QrApiResult? qrApiResult = await _blockchainService.GenerateQrCode();
            if (qrApiResult == null || qrApiResult.ReturnCode != "1")
            {
                LoggerHelper.LogError(new Exception(ErrorMessages.QrCodeError));
                return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
            }
            PdfSubmitApiResult? pdfSubmit = await _blockchainService.SubmitPdfToBlockchain(blobResult.Uri, qrApiResult.UniqueId!, metaData);
            if (pdfSubmit is null || pdfSubmit.ReturnCode != "1")
            {
                LoggerHelper.LogError(new Exception(pdfSubmit?.Message));
                return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
            }
            await Task.Delay(3000);
            BlockchainStatus? status = await _blockchainService.BlockchainStatus(qrApiResult.UniqueId, BlockchainStatusParam.UniqueId);
            while (status?.ReturnCode != "1")
            {
                await Task.Delay(3000);
                status = await _blockchainService.BlockchainStatus(qrApiResult.UniqueId, BlockchainStatusParam.UniqueId);
            }

            Property property = new()
            {
                UserId = propertyDto.UserId,
                Address = propertyDto.Address,
                LatLong = new NetTopologySuite.Geometries.Point(propertyDto.Lattitude, propertyDto.Longitude) { SRID = 4326  },
                Unit = propertyDto.Unit,
                Area = propertyDto.Area,
                PropertyTypeId = propertyDto.PropertyTypeId,
                Bedroom = propertyDto.Bedroom,
                YoutubeUrls = propertyDto.YoutubeUrl,
                VimeoUrls = propertyDto.VimeoeUrl,
                FloorPlanUrl = blobResult.Uri,
                Status = true,
                QrCode = qrApiResult.QrImage,
                QrLink = qrApiResult.QrLink,
                UniqueId = qrApiResult.UniqueId!,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BlockchainUrl = status?.BlockchainUrl,
                ParentBlockchainUrl = status?.ParentQrcode?.BlockchainUrl,
                TransectionId = status?.TransectionId
            };
            if (propertyDto.Video is not null)
            {
                fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + Path.GetExtension(propertyDto.Video?.FileName);
                blobResult = await _storageService.UploadFile(propertyDto.Video!, fileName, _config.PropertiesVideos);
                property.VideoUrl = blobResult.Uri;
                property.VideoThumb = propertyDto.VideoThumbnail;
            }
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();


            // upload images
            foreach (IFormFile item in propertyDto.ImageFiles)
            {
                fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + Path.GetExtension(item.FileName);
                BlobResult blob = await _storageService.UploadFile(item, fileName, _config.GalleryImages);
                propertyImages.Add(new PropertyImage()
                {
                    PropertyId = property.Id,
                    ClientName = item.Name,
                    Url = blob.Uri,
                    BlobName = blob.BlobName,
                    ImageType = PropertyImageType.GalleryImage
                });
            }

            foreach (IFormFile item in propertyDto.FloorPlanImageFiles)
            {
                fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + Path.GetExtension(item.FileName);
                BlobResult blob = await _storageService.UploadFile(item, fileName, _config.FloorImages);
                propertyImages.Add(new PropertyImage()
                {
                    PropertyId = property.Id,
                    ClientName = item.Name,
                    Url = blob.Uri,
                    BlobName = blob.BlobName,
                    //ImageType = PropertyImageType.FloorPlanImage
                });
            }

            await _dbContext.AddRangeAsync(propertyImages);
            await _dbContext.SaveChangesAsync();

            return new AppResult() { Success = true, Message = AppMessages.PropertyAddMessage };
        }

        public async Task<Property?> FindProperty(long id)
        {
            return await _dbContext.Properties.
               // Include(e => e.Images).
                Include(e => e.PropertyType).
                FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Property?> FindProperty(string uniqueId)
        {
            return await _dbContext.Properties.
                //Include(e => e.Images).
                Include(e => e.PropertyType).
                Include(e => e.PropertyContracts).
                Include(e => e.PropertyDocuments).
                FirstOrDefaultAsync(e => e.UniqueId == uniqueId);
        }

        public async Task<Property?> FindPropertyAlon(string uniqueId)
        {
            return await _dbContext.Properties.
                FirstOrDefaultAsync(e => e.UniqueId == uniqueId);
        }

        public async Task<int> FindPropertyMatch(string address)
        {
            return await _dbContext.Properties.Where(e => e.Address == address).CountAsync();
        }

        public async Task<List<Property>> GetProperties(string userId)
        {
            List<long> propertyIds = await _dbContext.UserInvites.
                Where(e => e.UserId == userId).
                Select(e => e.PropertyId).
                Distinct().
                ToListAsync();
            return await _dbContext.Properties.
                Where(e => e.UserId == userId || propertyIds.Contains(e.Id)).
                ToListAsync();
        }

        public async Task<List<Property>> GetProperties(string userId, int propertyTypeId, string address)
        {
            IQueryable<Property> properties = _dbContext.Properties.Include(e => e.User);
            if (propertyTypeId > 0)
            {
                properties = properties.Where(e => e.PropertyTypeId == propertyTypeId);
            }
            List<long> propertyIds = await _dbContext.UserInvites.
                Where(e => e.UserId == userId).
                Select(e => e.PropertyId).
                Distinct().
                ToListAsync();
            if (string.IsNullOrEmpty(address))
            {
                return await properties.
                                Where(e => e.UserId == userId || propertyIds.Contains(e.Id)).
                                ToListAsync();
            }
            
            return await properties.
                Where(e => e.UserId == userId || propertyIds.Contains(e.Id)).
                Where(e => e.Address.Contains(address)).
                ToListAsync();
        }

        public async Task<List<PropertyType>> GetPropertyTypesAsync()
        {
            return await _dbContext.PropertyTypes.OrderBy(e => e.Name).ToListAsync();
        }


        public async Task<List<PropertyEventCDto>> GetPropertyEvents(long id, string currentUserRole)
        {
            List<PropertyEventCDto> propertyEventCDtos = [];
            List<PropertyEvent> propertyEvents = await _dbContext.PropertyEvents.
                Include(e => e.PropertyDocument).
                Include(e => e.PropertyContract).
                Include(e => e.Event).
                Where(e => e.PropertyId == id).
                OrderBy(e => e.CreatedAt).
                ToListAsync();

            List<string> creatorIds = propertyEvents.Select(e => e.CreatedBy).Distinct().ToList();
            List<long?> contractIds = propertyEvents.Where(e => e.ContractId != null).Select(e => e.ContractId).Distinct().ToList();
            List<AppUserRole> strings = await _dbContext.Roles.Include(e => e.RoleMappings.Where(e => creatorIds.Contains(e.UserId))).ToListAsync();
            List<PropertyContract> contracts = await _dbContext.PropertyContracts.Where(e => contractIds.Contains(e.Id)).ToListAsync();

            string currentUserRoleId = strings.FirstOrDefault(e => e.Name == currentUserRole)?.Id!;

            propertyEvents.ForEach(e =>
            {
                List<string> rolesIds = JsonConvert.DeserializeObject<List<string>>(e.RoleVisibility)!;
                if (rolesIds.Contains(currentUserRoleId))
                {
                    string? roleid = strings.SelectMany(e => e.RoleMappings).Where(x => x.UserId == e.CreatedBy).FirstOrDefault()?.RoleId;
                    propertyEventCDtos.Add(new PropertyEventCDto()
                    {
                        RoleName = strings.FirstOrDefault(e => e.Id == roleid)?.Name,
                        EventType = e.Event?.Name,
                        ContractNumber = contracts.FirstOrDefault(x => x.Id == e.ContractId)?.Number,
                        EventDescription = e.EventDescription,
                        Url = e.DocumentUrl,
                        VerifyUrl = e.BlockchainUrl,
                        Date = e.CreatedAt,
                        QrUrl = e.PropertyDocument?.QrLink ?? e.PropertyContract?.QrLink ?? "",
                    });
                }
            });
            return propertyEventCDtos;
        }

        public async Task<List<PropertyEventCDto>> GetPropertyEventsAll(long id)
        {
            List<PropertyEventCDto> propertyEventCDtos = [];
            List<PropertyEvent> propertyEvents = await _dbContext.PropertyEvents.
                Include(e => e.PropertyDocument).
                Include(e => e.PropertyContract).
                Include(e => e.Event).
                Where(e => e.PropertyId == id).
                OrderBy(e => e.CreatedAt).
                ToListAsync();

            List<string> creatorIds = propertyEvents.Select(e => e.CreatedBy).Distinct().ToList();
            List<long?> contractIds = propertyEvents.Where(e => e.ContractId != null).Select(e => e.ContractId).Distinct().ToList();
            List<AppUserRole> strings = await _dbContext.Roles.Include(e => e.RoleMappings.Where(e => creatorIds.Contains(e.UserId))).ToListAsync();
            List<PropertyContract> contracts = await _dbContext.PropertyContracts.Where(e => contractIds.Contains(e.Id)).ToListAsync();            
            propertyEvents.ForEach(e =>
            {
                string? roleid = strings.SelectMany(e => e.RoleMappings).Where(x => x.UserId == e.CreatedBy).FirstOrDefault()?.RoleId;
                propertyEventCDtos.Add(new PropertyEventCDto()
                {
                    RoleName = strings.FirstOrDefault(e => e.Id == roleid)?.Name,
                    EventType = e.Event?.Name,
                    ContractNumber = contracts.FirstOrDefault(x => x.Id == e.ContractId)?.Number,
                    EventDescription = e.EventDescription,
                    Url = e.DocumentUrl,
                    VerifyUrl = e.BlockchainUrl,
                    Date = e.CreatedAt,
                    QrUrl = e.PropertyDocument?.QrLink ?? e.PropertyContract?.QrLink ?? "",
                });
            });
            return propertyEventCDtos;
        }

        #endregion


        #region Blockchain
        public async Task<QrApiResult?> GenerateQrCode()
        {
            return await _blockchainService.GenerateQrCode();
        }
        #endregion


        #region Property Contract
        public async Task<AppResult> AddContract(ContractDto contractDto)
        {
            Property? property = await _dbContext.Properties.FindAsync(contractDto.Property);
            EventType? events = await _dbContext.EventTypes.FirstOrDefaultAsync(e => e.NormalizedName == EventTypes.ContractUploaded.ToUpper().Replace(" ", "").Trim());
            if (property is not null && events is not null)
            {
                string rawData = $"Contract_Number: {contractDto.ContractNumber}||Owner_Name: {contractDto.OwnerName}||Previous_Title: {contractDto.PreviousTitle}";

                BlockchainTriggerResponse? triggerResponse = await _blockchainService.BlockchainTrigger(rawData, property.UniqueId);
                if (triggerResponse is null || triggerResponse.ReturnCode != "1")
                {
                    return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
                }

                Stream? stream = PdfSupport.AddQrToPdf(contractDto);
                string fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + ".pdf";
                BlobResult blobResult = await _storageService.UploadStream(stream!, fileName, _config.Contract);

                PdfSubmitApiResult? pdfSubmit = await _blockchainService.SubmitChildPdfToBlockchain(blobResult.Uri, contractDto.UniqueId, rawData, property.UniqueId, ChildDocumentType.ContractType);
                if (pdfSubmit is null || pdfSubmit.ReturnCode != "1")
                {
                    return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
                }
                BlockchainStatus? status =  await _blockchainService.BlockchainStatus(contractDto.UniqueId, BlockchainStatusParam.UniqueId);
                while (status?.Status != "Success")
                {
                    await Task.Delay(5000);
                    status = await _blockchainService.BlockchainStatus(contractDto.UniqueId, BlockchainStatusParam.UniqueId);
                }
                // change status of all previous contract to inactive
                List<PropertyContract> contracts = await _dbContext.PropertyContracts.Where(e => e.PropertyId == property.Id).ToListAsync();
                contracts.ForEach(e =>
                {
                    e.Status = false;
                });
                _dbContext.PropertyContracts.UpdateRange(contracts);
                await _dbContext.SaveChangesAsync();

                PropertyContract contract = new()
                {
                    PropertyId = property.Id,
                    Number = contractDto.ContractNumber,
                    Title = contractDto.PreviousTitle,
                    OwnerName = contractDto.OwnerName,
                    Date = contractDto.Date,
                    DocumentUrl = blobResult.Uri,
                    QrCode = contractDto.QR,
                    UniqueId = contractDto.UniqueId,
                    QrLink = contractDto.Qrlink,
                    BlockchainUrl = status.BlockchainUrl,
                    TransectionId = status.TransectionId,
                    FileHash = pdfSubmit.Filehash,
                    Guid = status.Guid,
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = contractDto.UserId
                };

                await _dbContext.PropertyContracts.AddAsync(contract);
                await _dbContext.SaveChangesAsync();


                status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                while (status?.Status != "Success")
                {
                    await Task.Delay(5000);
                    status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                }
                List<string> roleIds = await _dbContext.Roles.Select(e => e.Id).ToListAsync();

                PropertyEvent propertyEvent = new()
                {
                    PropertyId = property.Id,
                    CreatedBy = contractDto.UserId,
                    EventId = events.Id,
                    EventDescription = EventDescriptions.ContractUpload,
                    ContractId = contract.Id,
                    DocumentId = null,
                    DocumentUrl = blobResult.Uri,
                    RoleVisibility = JsonConvert.SerializeObject(roleIds),
                    BlockchainUrl = status.BlockchainUrl,
                    TransectionId = status.TransectionId,
                    Guid = status.Guid,
                    ParentBlockchainUrl = status.ParentQrcode.BlockchainUrl,
                    Status = BlockchainTrxStatus.Success,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.PropertyEvents.AddAsync(propertyEvent);
                await _dbContext.SaveChangesAsync();
                return new AppResult() { Success = true, Message = AppMessages.ContractBlockchainMessage };
            }
            return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
        }

        public async Task<List<PropertyContract>> FindPropertyContracts(long propertyId)
        {
            return await _dbContext.PropertyContracts.Where(e => e.PropertyId == propertyId).ToListAsync();
        }

        public async Task<PropertyContract?> FindPropertyContracts(string uniqueId)
        {
            return await _dbContext.PropertyContracts.
                Include(e => e.CreatedByUser).
                Include(e => e.Property).
                FirstOrDefaultAsync(e => e.UniqueId == uniqueId);
        }

        public async Task<PropertyContract?> FindPropertyActiveContracts(long propertyId)
        {
            return await _dbContext.PropertyContracts.
                FirstOrDefaultAsync(e => e.PropertyId == propertyId && e.Status);
        }

        public async Task<bool> RemoveContract(long id)
        {
            PropertyContract? propertyContract = await _dbContext.PropertyContracts.FindAsync(id);
            if(propertyContract is not null)
            {
                propertyContract.Status = false;
                _dbContext.PropertyContracts.Update(propertyContract);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion


        #region property Document
        public async Task<List<PropertyDocument>> FindPropertyDocument(long propertyId)
        {
            return await _dbContext.PropertyDocuments.Where(e => e.PropertyId == propertyId).ToListAsync();
        }

        public  async Task<PropertyDocument?> FindPropertyDocument(string uniqueId)
        {
            return await _dbContext.PropertyDocuments.
                Include(e => e.CreatedByUser).
                Include(e => e.Property).
                FirstOrDefaultAsync(e => e.UniqueId == uniqueId);
        }

        public async Task<AppResult> AddDocument(DocumentDto documentDto)
        {
            Property? property = await _dbContext.Properties.FindAsync(documentDto.Property);
            EventType? events = await _dbContext.EventTypes.FirstOrDefaultAsync(e => e.NormalizedName == EventTypes.Documentploaded.ToUpper().Replace(" ", "").Trim());
            if (property is not null && events is not null)
            {
                string rawData = $"Document_Label: {documentDto.Label}";

                BlockchainTriggerResponse? triggerResponse = await _blockchainService.BlockchainTrigger(rawData, property.UniqueId);
                if (triggerResponse is null || triggerResponse.ReturnCode != "1")
                {
                    return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
                }

                Stream? stream = PdfSupport.AddQrToPdf(documentDto);
                string fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + ".pdf";
                BlobResult blobResult = await _storageService.UploadStream(stream!, fileName, _config.PropertyDocument);

                PdfSubmitApiResult? pdfSubmit = await _blockchainService.SubmitChildPdfToBlockchain(blobResult.Uri, documentDto.UniqueId, rawData, property.UniqueId, ChildDocumentType.DocumentType);
                if (pdfSubmit is null || pdfSubmit.ReturnCode != "1")
                {
                    return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
                }
                BlockchainStatus? status = await _blockchainService.BlockchainStatus(documentDto.UniqueId, BlockchainStatusParam.UniqueId);
                while (status?.Status != "Success")
                {
                    await Task.Delay(5000);
                    status = await _blockchainService.BlockchainStatus(documentDto.UniqueId, BlockchainStatusParam.UniqueId);
                }
                
                PropertyDocument document = new()
                {
                    PropertyId = property.Id,
                    Label = documentDto.Label,
                    Date = documentDto.Date,
                    Url = blobResult.Uri,
                    QrCode = documentDto.QR,
                    UniqueId = documentDto.UniqueId,
                    QrLink = documentDto.Qrlink,
                    BlockchainUrl = status.BlockchainUrl,
                    TransectionId = status.TransectionId,
                    FileHash = pdfSubmit.Filehash,
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = documentDto.UserId
                };

                await _dbContext.PropertyDocuments.AddAsync(document);
                await _dbContext.SaveChangesAsync();


                status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                while (status?.Status != "Success")
                {
                    await Task.Delay(3000);
                    status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                }
                List<string> roleIds = await _dbContext.Roles.Select(e => e.Id).ToListAsync();

                PropertyEvent propertyEvent = new()
                {
                    PropertyId = property.Id,
                    CreatedBy = documentDto.UserId,
                    EventId = events.Id,
                    EventDescription = EventDescriptions.DocumentUpload,
                    ContractId = null,
                    DocumentId = document.Id,
                    DocumentUrl = blobResult.Uri,
                    RoleVisibility = JsonConvert.SerializeObject(roleIds),
                    BlockchainUrl = status.BlockchainUrl,
                    TransectionId = status.TransectionId,
                    Guid = status.Guid,
                    ParentBlockchainUrl = status.ParentQrcode.BlockchainUrl,
                    Status = BlockchainTrxStatus.Success,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.PropertyEvents.AddAsync(propertyEvent);
                await _dbContext.SaveChangesAsync();
                return new AppResult() { Success = true, Message = AppMessages.DocumentBlockchainMessage };
            }
            return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
        }

        public async Task<bool> RemoveDocument(long id)
        {
            PropertyDocument? propertyDocument = await _dbContext.PropertyDocuments.FindAsync(id);
            if (propertyDocument is not null)
            {
                propertyDocument.Status = false;
                _dbContext.PropertyDocuments.Update(propertyDocument);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion


        #region Invite
        public async Task<AppResult> InviteUser(UserInviteDto dto)
        {
            Property? property = await _dbContext.Properties.FindAsync(dto.Property);
            PropertyContract? propertyContract = await _dbContext.PropertyContracts.FindAsync(dto.Contract);
            AppUserRole? userRole = await _dbContext.Roles.FindAsync(dto.Role);
            EventType? events = await _dbContext.EventTypes.FirstOrDefaultAsync(e => e.NormalizedName == EventTypes.InviteToUser.ToUpper().Replace(" ", "").Trim());
            if (property is not null && userRole is not null && propertyContract is not null && events is not null)
            {
                string data = $"Invite to user: {dto.FirstName} {dto.LastName} || Email: {dto.Email} || Phone: {dto.PhoneCode} {dto.PhoneNumber}";
                BlockchainTriggerResponse? triggerResponse = await _blockchainService.BlockchainTrigger(data, property.UniqueId);
                if (triggerResponse is not null && triggerResponse.ReturnCode == "1")
                {
                    await Task.Delay(3000);
                    BlockchainStatus? status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                    while (status != null && status.ReturnCode != "1")
                    {
                        await Task.Delay(3000);
                        status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                    }
                    AppUser appUser = new ()
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Email = dto.Email,
                        UserName = dto.Email,
                        PhoneCode = dto.PhoneCode,
                        PhoneCodeId = dto.PhoneCodeId,
                        PhoneNumber = dto.PhoneNumber,
                        EmailConfirmed = true,
                        OrganizationId = dto.OrganisationId,
                        IsActive = true,
                    };
                    IdentityResult result = await _userManager.CreateAsync(appUser);
                    result = await _userManager.AddToRoleAsync(appUser, userRole.Name!);

                    UserInvite userInvite = new()
                    {
                        PropertyId = property.Id,
                        ContractId = propertyContract.Id,
                        UserId = appUser.Id,
                        RoleName = userRole.Name!,
                        InvitedBy = dto.UserId,
                        CreateAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _dbContext.UserInvites.AddAsync(userInvite);
                    await _dbContext.SaveChangesAsync();

                    List<string> roleIds = await _dbContext.Roles.Select(e => e.Id).ToListAsync();
                    

                    PropertyEvent propertyEvent = new()
                    {
                        PropertyId = property.Id,
                        CreatedBy = dto.UserId,
                        EventId = events?.Id,
                        EventDescription = EventDescriptions.UserInvite,
                        ContractId = null,
                        DocumentUrl = null,
                        RoleVisibility = JsonConvert.SerializeObject(roleIds),
                        BlockchainUrl = status?.BlockchainUrl,
                        TransectionId = status?.TransectionId,
                        Guid = status?.Guid,
                        ParentBlockchainUrl = status?.ParentQrcode?.BlockchainUrl,
                        Status = BlockchainTrxStatus.Success,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _dbContext.PropertyEvents.AddAsync(propertyEvent);
                    await _dbContext.SaveChangesAsync();

                    await _mailService.SendInvite(new EmailDto() 
                    { 
                        Email = dto.Email,
                        UserName = $"{dto.FirstName} {dto.LastName}",
                        Link = property.QrLink
                    });

                    return new AppResult() { Success = true, Message = AppMessages.UserInviteMessage };
                }
                
            }
            return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
        }

        public async Task<List<UserInvite>> FindUserInvite(long propertyId)
        {
            return await _dbContext.UserInvites.
                Include(e => e.User).
                ThenInclude(e => e.UserCredentials).
                Where(e => e.PropertyId == propertyId).
                ToListAsync();
        }

        public async Task<List<UserInviteCustomDto>> FindUserInviteByContract(long contractId)
        {
            List<UserInvite> userInvites = await _dbContext.UserInvites.
                Include(e => e.User).
                ThenInclude(e => e!.UserRoleMappings)!.
                ThenInclude(e => e.AppUserRole).
                Where(e => e.ContractId == contractId).
                ToListAsync();
            List<UserInviteCustomDto> inviteCustomDtos = [];
            userInvites.ForEach(e =>
            {
                inviteCustomDtos.Add(new UserInviteCustomDto()
                {
                    RoleId = e.User?.UserRoleMappings?.FirstOrDefault()?.AppUserRole.Id,
                    RoleName = e.User?.UserRoleMappings?.FirstOrDefault()?.AppUserRole.Name,
                    NormalizedRoleName = e.User?.UserRoleMappings?.FirstOrDefault()?.AppUserRole.NormalizedName,
                });
            });

            return inviteCustomDtos;
        }
        #endregion


        #region Events management
        public async Task<bool> AddEvent(string eventName, string createdByUserId)
        {
            EventType @event = new()
            {
                Name = eventName,
                NormalizedName = eventName.Trim().Replace(" ","").ToUpper(),
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.EventTypes.AddAsync(@event);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<AppResult> SaveCustomEvent(ManagePropertyDto dto)
        {
            Property? property = await _dbContext.Properties.FindAsync(dto.Property);
            EventType? events = await _dbContext.EventTypes.FindAsync(dto.EventId);
            PropertyContract? propertyContract = await _dbContext.PropertyContracts.FindAsync(dto.Contract);
            if (property is not null && events is not null && propertyContract is not null)
            {
                string rawData = $"Contract_Number: {propertyContract.Number}| Description: {dto.Description}| Event_name: {events.Name}";
                BlockchainTriggerResponse? triggerResponse = await _blockchainService.BlockchainTrigger(rawData, property.UniqueId);
                if (triggerResponse is null || triggerResponse.ReturnCode != "1")
                {
                    return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
                }
                string fileName = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Ticks + ".pdf";
                BlobResult blobResult = await _storageService.UploadFile(dto.File!, fileName, _config.CustomEventDoc);

                BlockchainStatus? status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                while (status?.Status != "Success")
                {
                    await Task.Delay(3000);
                    status = await _blockchainService.BlockchainStatus(triggerResponse.Guid, BlockchainStatusParam.Guid);
                }

                PropertyEvent propertyEvent = new()
                {
                    PropertyId = property.Id,
                    CreatedBy = dto.UserId,
                    EventId = events.Id,
                    EventDescription = dto.Description,
                    ContractId = dto.Contract,
                    DocumentId = null,
                    DocumentUrl = blobResult.Uri,
                    RoleVisibility = dto.RoleVisibility,
                    BlockchainUrl = status.BlockchainUrl,
                    TransectionId = status.TransectionId,
                    Guid = status.Guid,
                    ParentBlockchainUrl = status.ParentQrcode.BlockchainUrl,
                    Status = BlockchainTrxStatus.Success,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.PropertyEvents.AddAsync(propertyEvent);
                await _dbContext.SaveChangesAsync();

                List<string> rolesIds = JsonConvert.DeserializeObject<List<string>>(dto.RoleVisibility)!;
                List<string?> rolesNames = await _dbContext.Roles.Where(e => rolesIds.Contains(e.Id)).Select(e => e.Name).ToListAsync();
                List<UserCustomDto> userCustomDtos = await _dbContext.UserInvites.Include(e => e.User).
                    Where(e => rolesNames.Contains(e.RoleName)).
                    Where(e => e.ContractId == dto.Contract).
                    Select(e => new UserCustomDto()
                    {
                        FirstName = e.User!.FirstName,
                        LastName = e.User!.LastName,
                        Email = e.User!.Email
                    }).
                    ToListAsync();
                await _mailService.SendContractInviteEmail(new EmailDto()
                {
                    Emails = userCustomDtos,
                    Subject = "VeriBuild Contract Event Invite",
                    PropertName = property.Address,
                    ContractNnumber = propertyContract.Number,
                    EventName = events.Name
                });

                return new AppResult() { Success = true, Message = AppMessages.EventBlockchainMessage };
            }
            return new AppResult() { Success = false, Message = ErrorMessages.Error500 };
        }

        public async Task<EventType?> FindEventAsync(string eventName)
        {
            return await _dbContext.EventTypes.FirstOrDefaultAsync(e => e.Name == eventName);
        }

        public async Task<EventType?> FindEventAsync(int eventId)
        {
            return await _dbContext.EventTypes.FindAsync(eventId);
        }

        public async Task<List<EventType>> GetEventsAsync()
        {
            return await _dbContext.EventTypes.
                Where(e => e.Name != EventTypes.InviteToUser && 
                    e.Name != EventTypes.Documentploaded &&
                    e.Name != EventTypes.ContractUploaded).
                OrderBy(e => e.Name).
                ToListAsync();
        }

        #endregion

    }


}
