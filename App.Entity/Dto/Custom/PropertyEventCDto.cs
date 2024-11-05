namespace App.Entity.Dto.Custom
{
    public class PropertyEventCDto
    {
        public string? RoleName { get; set; }
        public string? EventType { get; set; }
        public string? ContractNumber { get; set; }
        public string? EventDescription { get; set; }
        public string? Url { get; set; }
        public string? VerifyUrl { get; set; }
        public string? DocumentQrUrl { get; set; }
        public string? QrUrl { get; set; }
        public DateTime Date { get; set; }

    }
}
