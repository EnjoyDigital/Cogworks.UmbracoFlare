using Cogworks.UmbracoFlare.Core.Extensions;
using Cogworks.UmbracoFlare.Core.Helpers;
using Cogworks.UmbracoFlare.Core.Model;
using Cogworks.UmbracoFlare.Core.Models;
using Cogworks.UmbracoFlare.Core.Models.Api;
using Cogworks.UmbracoFlare.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;

namespace Cogworks.UmbracoFlare.Core.Controllers
{
    [PluginController("UmbracoFlare")]
    public class CloudflareUmbracoApiController : UmbracoApiController
    {
        private readonly ICloudflareService cloudflareService;
        private readonly IUmbracoFlareDomainService domainService;
        private readonly IUmbracoFlareUrlService urlService;
        private readonly IConfigurationService configurationService;

        public CloudflareUmbracoApiController(ICloudflareService cloudflareService, IConfigurationService configurationService, IUmbracoFlareDomainService domainService, IUmbracoFlareUrlService urlService)
        {
            this.cloudflareService = cloudflareService;
            this.configurationService = configurationService;
            this.domainService = domainService;
            this.urlService = urlService;
        }

        [HttpGet]
        public UmbracoFlareConfigModel GetConfig()
        {
            var configurationFile = configurationService.LoadConfigurationFile();
            if (!configurationService.ConfigurationFileHasData(configurationFile))
            {
                return configurationFile;
            }

            var userDetails = cloudflareService.GetCloudflareUserDetails();
            configurationFile.CredentialsAreValid = userDetails != null && userDetails.Success;
            configurationFile.AllowedDomains = domainService.GetAllowedCloudflareDomains();

            return configurationFile;
        }

        [HttpPost]
        public UmbracoFlareConfigModel UpdateConfigStatus([FromBody] UmbracoFlareConfigModel config)
        {
            var configurationFile = configurationService.SaveConfigurationFile(config);
            var userDetails = cloudflareService.GetCloudflareUserDetails();
            configurationFile.CredentialsAreValid = userDetails != null && userDetails.Success;

            configurationFile = configurationService.SaveConfigurationFile(config);

            return configurationFile;
        }

        [HttpPost]
        public StatusWithMessage PurgeAll([FromQuery]string currentDomain)
        {
            var currentDomainIsValid = domainService.GetAllowedCloudflareDomains().Count(x => x.Equals(currentDomain)) > 0;

            if (!currentDomainIsValid)
            {
                return new StatusWithMessage(false, "The current domain is not valid, please check if the domain is a valid zone in your cloudflare account.");
            }

            var result = cloudflareService.PurgeEverything(currentDomain);
            return result;
        }

        [HttpPost]
        public StatusWithMessage PurgeStaticFiles([FromBody] PurgeStaticFilesRequestModel model)
        {
            if (!model.StaticFiles.HasAny())
            {
                return new StatusWithMessage(false, "There were not static files selected to purge");
            }

            var currentDomainIsValid = domainService.GetAllowedCloudflareDomains().Count(x => x.Equals(model.CurrentDomain)) > 0;

            if (!currentDomainIsValid)
            {
                return new StatusWithMessage(false, "The current domain is not valid, please check if the domain is a valid zone in your cloudflare account.");
            }
            
            var fullUrlsToPurge = new List<string>();
            var allFilePaths = cloudflareService.GetFilePaths(model.StaticFiles);

            foreach (var filePath in allFilePaths)
            {
                var extension = Path.GetExtension(filePath);

                if (ApplicationConstants.AllowedFileExtensions.Contains(extension))
                {
                    var urls = urlService.GetFullUrlForPurgeStaticFiles(filePath, model.CurrentDomain, true);
                    fullUrlsToPurge.AddRange(urls);
                }
            }

            var result = cloudflareService.PurgePages(fullUrlsToPurge);
            
            return !result.Success ? result : new StatusWithMessage(true, $"{fullUrlsToPurge.Count()} static files were purged successfully.");
        }

        [HttpPost]
        public StatusWithMessage PurgeCacheForUrls([FromBody] PurgeUrlsRequestModel model)
        {
            if (!model.Urls.HasAny())
            {
                return new StatusWithMessage(false, "You must provide urls to clear the cache for.");
            }

            var currentDomainIsValid = domainService.GetAllowedCloudflareDomains().Count(x => x.Equals(model.CurrentDomain)) > 0;

            if (!currentDomainIsValid)
            {
                return new StatusWithMessage(false, "The current domain is not valid, please check if the domain is a valid zone in your cloudflare account.");
            }
            
            var builtUrls = new List<string>();
            builtUrls.AddRange(urlService.MakeFullUrlsWithDomain(model.Urls, model.CurrentDomain, true));
            
            var urlsWithWildCards = builtUrls.Where(x => x.Contains('*'));
            var willCardsUrls = !urlsWithWildCards.HasAny()
                ? builtUrls
                : domainService.GetAllUrlsForWildCardUrls(urlsWithWildCards);

            builtUrls.AddRangeUnique(willCardsUrls);

            var result = cloudflareService.PurgePages(builtUrls);

            return !result.Success ? result : new StatusWithMessage(true, $"{builtUrls.Count()} urls purged successfully.");
        }

        [HttpPost]
        public StatusWithMessage PurgeCacheForContentNode([FromBody] PurgeFromContentTree model)
        {
            if (model.NodeId <= 0)
            {
                return new StatusWithMessage(false, "You must provide a node id.");
            }

            var currentDomainIsValid = domainService.GetAllowedCloudflareDomains().Count(x => x.Equals(model.CurrentDomain)) > 0;

            if (!currentDomainIsValid)
            {
                return new StatusWithMessage(false, "The current domain is not valid, please check if the domain is a valid zone in your cloudflare account " +
                                                    "and make sure you this account is associated in the umbracoflare dashboard");
            }

            var urls = new List<string>();
            urls.AddRange(domainService.GetUrlsForNode(model.NodeId, model.CurrentDomain, model.PurgeChildren));

            var result = cloudflareService.PurgePages(urls);

            return !result.Success ? result : new StatusWithMessage(true, $"{urls.Count()} urls purged successfully.");
        }
    }
}