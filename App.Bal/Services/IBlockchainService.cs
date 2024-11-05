using App.Entity.Http;
using App.Foundation.Enumeration;

namespace App.Bal.Services
{
    public interface IBlockchainService
    {
        public Task<QrApiResult?> GenerateQrCode();
        public Task<PdfSubmitApiResult?> SubmitPdfToBlockchain(string pdfUrl, string uniqueId, string data);
        public Task<PdfSubmitApiResult?> SubmitChildPdfToBlockchain(string pdfUrl, string uniqueId, string metadata, string parentUniqueId, ChildDocumentType documentType);
        public Task<BlockchainTriggerResponse?> BlockchainTrigger(string data, string uniqueId, byte isChild = 1);
        public Task<BlockchainStatus?> BlockchainStatus(string? guid, BlockchainStatusParam statusParam);
    }
}
