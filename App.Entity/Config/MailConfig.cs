namespace App.Entity.Config
{
    public class MailConfig
    {
        public const string Path = "EmailConfig";
        public string Email { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
