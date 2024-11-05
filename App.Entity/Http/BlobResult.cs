namespace App.Entity.Http
{
    public class BlobResult
    {
        public string BlobContainerName { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
    }
}
