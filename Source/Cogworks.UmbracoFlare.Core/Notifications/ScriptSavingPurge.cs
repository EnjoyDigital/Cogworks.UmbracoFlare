using Cogworks.UmbracoFlare.Core.Model;
using Cogworks.UmbracoFlare.Core.Helpers;
using Cogworks.UmbracoFlare.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Cogworks.UmbracoFlare.Core.Notifications
{
    public class ScriptSavingPurge : INotificationHandler<ScriptSavingNotification>
    {
        private readonly IConfigurationService configurationService;
        private readonly ICloudflareService cloudflareService;
        private readonly IUmbracoFlareUrlService urlService;

        public ScriptSavingPurge(IConfigurationService configurationService, ICloudflareService cloudflareService, IUmbracoFlareUrlService urlService)
        {
            this.configurationService = configurationService;
            this.cloudflareService = cloudflareService;
            this.urlService = urlService;
        }

        public void Handle(ScriptSavingNotification notification)
        {
            var umbracoFlareConfigModel = configurationService.LoadConfigurationFile();
            if (!umbracoFlareConfigModel.PurgeCacheOn) { return; }
            if (!notification.SavedEntities.Any()) { return; }

            var currentDomain = urlService.GetCurrentDomain();
            var urls = new List<string>();

            foreach (var file in notification.SavedEntities)
            {
                if (!file.HasIdentity) { continue; }
                urls.Add(file.VirtualPath);
            }

            var fullUrls = urlService.MakeFullUrlsWithDomain(urls, currentDomain, true);

            var result = cloudflareService.PurgePages(fullUrls);

            notification.Messages.Add(result.Success
                ? new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching, "Successfully purged the cloudflare cache.",
                    EventMessageType.Success)
                : new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching, "We could not purge the Cloudflare cache.",
                    EventMessageType.Warning));
        }
    }
}