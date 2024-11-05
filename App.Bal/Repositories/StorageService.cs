using App.Bal.Services;
using App.Entity.Config;
using App.Entity.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace App.Bal.Repositories
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly AzureConfig _azureConfig;

        public StorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _azureConfig = new();
            _configuration.GetSection(AzureConfig.Key).Bind(_azureConfig);
        }

        public async Task<bool> DeleteFile(string blobName)
        {
            BlobContainerClient blobContainerClient = new (_azureConfig.ConnectionString, _azureConfig.ContainerName);
            bool blobContainerExist = await blobContainerClient.ExistsAsync();
            if (blobContainerExist)
            {
                BlobClient client = blobContainerClient.GetBlobClient(blobName);
                return await client.DeleteIfExistsAsync();
            }
            return false;
        }

        public async Task<BlobResult> UploadBytes(byte[] stream, string fileName, string folderName)
        {
            BlobContainerClient containerClient = new(_azureConfig.ConnectionString, _azureConfig.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            string blobName = $"{folderName}{fileName}";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(new BinaryData(stream));
            BlobResult blobResult = new()
            {
                Uri = blobClient.Uri.AbsoluteUri,
                BlobName = blobClient.Name,
                BlobContainerName = blobClient.BlobContainerName
            };
            return blobResult;
        }

        public async Task<BlobResult> UploadFile(IFormFile file, string fileName, string folderName)
        {
            BlobContainerClient containerClient = new(_azureConfig.ConnectionString, _azureConfig.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            string blobName = $"{folderName}{fileName}";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using Stream stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream);
            BlobResult blobResult = new()
            {
                Uri = blobClient.Uri.AbsoluteUri,
                BlobName = blobClient.Name,
                BlobContainerName = blobClient.BlobContainerName
            };
            return blobResult;
        }

        public async Task<BlobResult> UploadStream(Stream stream, string fileName, string folderName)
        {
            BlobContainerClient containerClient = new(_azureConfig.ConnectionString, _azureConfig.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            string blobName = $"{folderName}{fileName}";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(stream);
            BlobResult blobResult = new()
            {
                Uri = blobClient.Uri.AbsoluteUri,
                BlobName = blobClient.Name,
                BlobContainerName = blobClient.BlobContainerName
            };
            return blobResult;
        }
    }
}
