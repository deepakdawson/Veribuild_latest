using App.Entity.Http;
using Microsoft.AspNetCore.Http;

namespace App.Bal.Services
{
    public interface IStorageService
    {
        public Task<BlobResult> UploadFile(IFormFile file, string fileName, string folderName);
        public Task<BlobResult> UploadStream(Stream stream, string fileName, string folderName);
        public Task<BlobResult> UploadBytes(byte[] stream, string fileName, string folderName);
        public Task<bool> DeleteFile(string blobName);
    }
}
