namespace App.Entity.Config
{
    public class BlockchainConfig
    {
        public const string Key = "BlockchainConfig";

        public const string QrGenerateUrl = "https://veridocglobal.com/api/generateqr";
        public const string SubmitPdfUrl = "https://veridocglobal.com/api/submitdocument";
        public const string UpdatePdfUrl = "https://my.veridocglobal.com/api/update";
        public const string BlockchainTriggerUrl = "https://my.veridocglobal.com/api/blockchaintxtrigger";
        public const string BlockchainStatusUrl = "https://my.veridocglobal.com/api/getblockchainstatus";
        public const string RedirectUrlUrl = "https://my.veribuild.com.au/properties/details/";
        public const string ContractRedirectUrl = "https://my.veribuild.com.au/properties/contracts/";
        public const string DocumentRedirectUrl = "https://my.veribuild.com.au/properties/documents/";

        public string ApiKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
