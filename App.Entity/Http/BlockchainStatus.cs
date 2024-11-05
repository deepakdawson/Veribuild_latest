using Newtonsoft.Json;

namespace App.Entity.Http
{
    public class BlockchainStatus
    {
        [JsonProperty("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonProperty("transactionid")]
        public string TransectionId { get; set; } = string.Empty;

        [JsonProperty("blockchainstatus")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("returncode")]
        public string ReturnCode { get; set; } = string.Empty;

        [JsonProperty("blockchainurl")]
        public string BlockchainUrl { get; set; } = string.Empty;

        [JsonProperty("parentqrcode")]
        public ParentQrcode ParentQrcode { get; set; } = new ParentQrcode();
    }

    public class ParentQrcode
    {
        [JsonProperty("verifyurl")]
        public string VerifyLink { get; set; } = string.Empty;

        [JsonProperty("blockchainurl")]
        public string BlockchainUrl { get; set; } = string.Empty;

        [JsonProperty("transactionid")]
        public string TransectionId { get; set; } = string.Empty;

        [JsonProperty("blockchainstatus")]
        public string Status { get; set; } = string.Empty;
    }
}
