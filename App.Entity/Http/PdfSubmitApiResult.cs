using Newtonsoft.Json;

namespace App.Entity.Http
{
    public class PdfSubmitApiResult
    {
        [JsonProperty("filehash")]
        public string? Filehash { get; set; }


        [JsonProperty("returncode")]
        public string? ReturnCode { get; set; }


        [JsonProperty("uniqueid")]
        public string? UniqueId { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
