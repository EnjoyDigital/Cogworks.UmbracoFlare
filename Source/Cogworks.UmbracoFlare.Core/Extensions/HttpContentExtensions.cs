using Cogworks.UmbracoFlare.Core.Model;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cogworks.UmbracoFlare.Core.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content, JsonSerializerOptions options = null)
        {
            using (Stream contentStream = await content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(contentStream, options ?? ApplicationConstants.DefaultJsonSerializerOptions);
            }
        }
    }
}