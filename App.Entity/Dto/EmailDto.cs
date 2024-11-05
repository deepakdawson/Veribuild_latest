using App.Entity.Dto.Custom;

namespace App.Entity.Dto
{
    public class EmailDto
    {
        public List<UserCustomDto> Emails { get; set; } = [];
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Link { get; set; }
        public string? Subject { get; set; }
        public string? OTP { get; set; }
        public string? PropertName { get; set; }
        public string? EventName { get; set; }
        public string? ContractNnumber { get; set; }

    }
}
