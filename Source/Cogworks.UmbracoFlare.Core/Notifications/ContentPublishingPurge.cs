using Cogworks.UmbracoFlare.Core.Model;
using Cogworks.UmbracoFlare.Core.Services;
using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Cogworks.UmbracoFlare.Core.Notifications
{
    public class ContentPublishingPurge : INotificationHandler<ContentPublishingNotification>
    {
        private readonly IConfigurationService configurationService;
        private readonly IUmbracoFlareDomainService umbracoFlareDomainService;
        private readonly ICloudflareService cloudflareService;
        private readonly IUmbracoFlareUrlService urlService;

        public ContentPublishingPurge(
            IConfigurationService configurationService, IUmbracoFlareDomainService umbracoFlareDomainService, ICloudflareService cloudflareService, IUmbracoFlareUrlService urlService)
        {
            this.configurationService = configurationService;
            this.umbracoFlareDomainService = umbracoFlareDomainService;
            this.cloudflareService = cloudflareService;
            this.urlService = urlService;
        }

        public void Handle(ContentPublishingNotification notification)
        {
            var umbracoFlareConfigModel = configurationService.LoadConfigurationFile();
            if (!umbracoFlareConfigModel.PurgeCacheOn) { return; }

            var urls = new List<string>();
            var currentDomain = urlService.GetCurrentDomain();

            foreach (var content in notification.PublishedEntities)
            {
                if (content.GetValue<bool>(ApplicationConstants.UmbracoFlareBackendProperties.CloudflareDisabledOnPublishPropertyAlias)) { continue; }

                urls.AddRange(umbracoFlareDomainService.GetUrlsForNode(content.Id, currentDomain));
            }

            var result = cloudflareService.PurgePages(urls);

            notification.Messages.Add(result.Success
                ? new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching,
                "Successfully purged the cloudflare cache.", EventMessageType.Success)
                : new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching,
                    "We could not purge the Cloudflare cache. Please check the logs to find out more.",
                    EventMessageType.Warning));
        }
    }
}