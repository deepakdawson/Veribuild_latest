using App.Bal.Services;
using App.Entity.Config;
using App.Entity.Http;
using App.Foundation.Common;
using App.Foundation.Enumeration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace App.Bal.Repositories
{
    public class BlockchainService : IBlockchainService
    {
        private readonly BlockchainConfig _config;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public BlockchainService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _config = new ();
            _configuration.GetSection(BlockchainConfig.Key).Bind(_config);
        }


        public async Task<BlockchainStatus?> BlockchainStatus(string? uniqieId, BlockchainStatusParam statusParam) 
        {
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                string rawData = _config.ApiKey + uniqieId + _config.SecretKey;
                string payload = Crypto.ComputeSha256Hash(rawData);
                client.DefaultRequestHeaders.Add("apikey", _config.ApiKey);
                client.DefaultRequestHeaders.Add("payload", payload);
                object ob;
                if (statusParam == BlockchainStatusParam.Guid) {
                    ob = new { guid = uniqieId };
                }
                else
                {
                    ob = new { uniqueid = uniqieId };
                }
                JsonContent jsonContent = JsonContent.Create(ob);
                HttpResponseMessage httpResponse = await client.PostAsync(BlockchainConfig.BlockchainStatusUrl, jsonContent);
                if (httpResponse.IsSuccessStatusCode)
                {
                    BlockchainStatus? result = JsonConvert.DeserializeObject<BlockchainStatus>(await httpResponse.Content.ReadAsStringAsync());
                    return result;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
            }
            return null;
        }


        public async Task<BlockchainTriggerResponse?> BlockchainTrigger(string data, string uniqueId, byte isChild = 1)
        {
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                string rawData = _config.ApiKey + data + isChild.ToString() + uniqueId + _config.SecretKey;
                string payload = Crypto.ComputeSha256Hash(rawData);
                client.DefaultRequestHeaders.Add("apikey", _config.ApiKey);
                client.DefaultRequestHeaders.Add("payload", payload);
                JsonContent jsonContent = JsonContent.Create(new { blockchaindata = data, ischild = isChild, parentid = uniqueId });
                HttpResponseMessage httpResponse = await client.PostAsync(BlockchainConfig.BlockchainTriggerUrl, jsonContent);
                if (httpResponse.IsSuccessStatusCode)
                {
                    BlockchainTriggerResponse? result = JsonConvert.DeserializeObject<BlockchainTriggerResponse>(await httpResponse.Content.ReadAsStringAsync());
                    return result;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
            }
            return null;
        }


        public async Task<QrApiResult?> GenerateQrCode()
        {
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                string payload = Crypto.ComputeSha256Hash(_config.ApiKey + _config.SecretKey);
                client.DefaultRequestHeaders.Add("apikey", _config.ApiKey);
                client.DefaultRequestHeaders.Add("payload", payload);

                StringContent stringContent = new("");
                client.Timeout = TimeSpan.FromSeconds(20);
                HttpResponseMessage httpResponse = await client.PostAsync(BlockchainConfig.QrGenerateUrl, stringContent);
                string message = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<QrApiResult>(message);
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
            }
            return null;
        }


        public async Task<PdfSubmitApiResult?> SubmitPdfToBlockchain(string pdfUrl, string qrUniqueId, string data)
        {
            try
            {
                string isFileUrlPublic = "1";
                using HttpClient client = _httpClientFactory.CreateClient();
                string rawData = _config.ApiKey + qrUniqueId + pdfUrl + isFileUrlPublic + data + "||" + ":" + BlockchainConfig.RedirectUrlUrl + qrUniqueId + _config.SecretKey;
                string payload = Crypto.ComputeSha256Hash(rawData);
                client.DefaultRequestHeaders.Add("apikey", _config.ApiKey);
                client.DefaultRequestHeaders.Add("payload", payload);
                JsonContent jsonContent = JsonContent.Create(new 
                { 
                    uniqueId = qrUniqueId, 
                    fileurl = pdfUrl, 
                    isfileurlpublic = isFileUrlPublic,
                    metadata = data,
                    parent_delimiter = "||",
                    child_delimiter = ":",
                    Redirecturl = BlockchainConfig.RedirectUrlUrl + qrUniqueId 
                });
                HttpResponseMessage httpResponse = await client.PostAsync(BlockchainConfig.SubmitPdfUrl, jsonContent);
                if (httpResponse.IsSuccessStatusCode)
                {
                    PdfSubmitApiResult? result = JsonConvert.DeserializeObject<PdfSubmitApiResult>(await httpResponse.Content.ReadAsStringAsync());
                    return result;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
            }
            return null;
        }


        public async Task<PdfSubmitApiResult?> SubmitChildPdfToBlockchain(string pdfUrl, string qrUniqueId, string metadata, string parentUniqueId, ChildDocumentType documentType)
        {
            try
            {
                string redirectUrl = documentType == ChildDocumentType.ContractType ? BlockchainConfig.ContractRedirectUrl : BlockchainConfig.DocumentRedirectUrl;
                string isFileUrlPublic = "1";
                string isParent = "0";
                using HttpClient client = _httpClientFactory.CreateClient();
                string rawData = _config.ApiKey + qrUniqueId + pdfUrl + isFileUrlPublic + metadata + "||" + ":" + redirectUrl + qrUniqueId + isParent + parentUniqueId + _config.SecretKey;
                string payload = Crypto.ComputeSha256Hash(rawData);
                client.DefaultRequestHeaders.Add("apikey", _config.ApiKey);
                client.DefaultRequestHeaders.Add("payload", payload);
                JsonContent jsonContent = JsonContent.Create(new
                {
                    uniqueId = qrUniqueId,
                    fileurl = pdfUrl,
                    isfileurlpublic = isFileUrlPublic,
                    metadata,
                    parent_delimiter = "||",
                    child_delimiter = ":",
                    Redirecturl = redirectUrl + qrUniqueId,
                    isparent = isParent,
                    parentid = parentUniqueId
                });
                HttpResponseMessage httpResponse = await client.PostAsync(BlockchainConfig.SubmitPdfUrl, jsonContent);
                if (httpResponse.IsSuccessStatusCode)
                {
                    PdfSubmitApiResult? result = JsonConvert.DeserializeObject<PdfSubmitApiResult>(await httpResponse.Content.ReadAsStringAsync());
                    return result;
                }
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e);
            }
            return null;
        }
    }
}
