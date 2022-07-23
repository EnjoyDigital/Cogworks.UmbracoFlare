using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cogworks.UmbracoFlare.Core.Models.Cloudflare
{
    public class BasicCloudflareResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("errors")]
        public List<CloudflareError> Errors { get; set; }

        [JsonPropertyName("messages")]
        public List<string> Messages { get; set; }
    }
}
