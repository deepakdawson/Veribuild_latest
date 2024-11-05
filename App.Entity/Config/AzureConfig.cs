namespace App.Entity.Config
{
    public class AzureConfig
    {
        public const string Key = "AzureStorage";
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
        public int TokenExpirationMinutes { get; set; }
    }

}
