using System;
using System.Text.Json.Serialization;

namespace Cogworks.UmbracoFlare.Core.Models.Cloudflare
{
    public class UserDetails : BasicCloudflareResponse
    {
        [JsonPropertyName("result")]
        public UserDetailResult Result { get; set; }
    }

    public class UserDetailResult
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName{ get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("telephone")]
        public string Telephone { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("zipcode")]
        public string Zipcode { get; set; }

        [JsonPropertyName("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modified_on")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; }

        [JsonPropertyName("two_factor_authentication_enabled")]
        public bool TwoFactorAuthenticationEnabled { get; set; }
    }
}
