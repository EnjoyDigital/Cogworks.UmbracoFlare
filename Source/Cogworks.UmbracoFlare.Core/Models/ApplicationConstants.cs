﻿using System.Text.Json;

namespace Cogworks.UmbracoFlare.Core.Model
{
    public static class ApplicationConstants
    {
        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public const string ContentTypeApplicationJson = "application/json";
        public static readonly string[] AllowedFileExtensions = { ".css", ".js", ".jpg", ".png", ".gif", ".aspx", ".html" };
        public const string ImageCropperPropertyEditorAlias = "Umbraco.ImageCropper";

        public static class UmbracoFlareBackendProperties
        {
            public const string CloudflareDisabledOnPublishPropertyAlias = "cloudflareDisabledOnPublish";
        }

        public static class EventMessageCategory
        {
            public const string CloudflareCaching = "Cloudflare Caching";
        }

        public static class ConfigurationFile
        {
            public const string ConfigurationFilePath = "~/Config/cloudflare.config";
        }

        public static class CloudflareApi
        {
            public const string BaseUrl = "https://api.cloudflare.com/client/v4/";
            public const string UserEndpoint = "user";
            public const string ZonesEndpoint = "zones";
        }
    }
}