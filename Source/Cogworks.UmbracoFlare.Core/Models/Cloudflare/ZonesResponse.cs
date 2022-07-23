using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cogworks.UmbracoFlare.Core.Models.Cloudflare
{
    public class ZonesResponse : BasicCloudflareResponse
    {
        [JsonPropertyName("result")]
        public IEnumerable<Zone> Zones { get; set; }
    }
}