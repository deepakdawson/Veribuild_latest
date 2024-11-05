using Newtonsoft.Json;

namespace App.Entity.Http
{
    public class QrApiResult
    {
        [JsonProperty("returncode")]
        public string? ReturnCode { get; set; }


        [JsonProperty("qrImage")]
        public string? QrImage { get; set; }


        [JsonProperty("uniqueId")]
        public string? UniqueId { get; set; }


        [JsonProperty("qr")]
        public string? QrLink { get; set; }
    }
}
