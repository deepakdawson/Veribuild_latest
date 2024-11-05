using Newtonsoft.Json;

namespace App.Entity.Http
{
    public class BlockchainTriggerResponse
    {
        [JsonProperty("guid")]
        public string Guid { get; set; } = string.Empty;

        [JsonProperty("returncode")]
        public string ReturnCode { get; set; } = string.Empty;
    }
}
