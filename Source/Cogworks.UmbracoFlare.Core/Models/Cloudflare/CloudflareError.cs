using System.Text.Json.Serialization;

namespace Cogworks.UmbracoFlare.Core.Models.Cloudflare
{
    public class CloudflareError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
