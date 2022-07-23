using Cogworks.UmbracoFlare.Core.Model;
using Cogworks.UmbracoFlare.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Cogworks.UmbracoFlare.Core.Notifications
{
    public class MediaSavedPurge : INotificationHandler<MediaSavedNotification>
    {
        private readonly IImageCropperService imageCropperService;
        private readonly IUmbracoContextFactory umbracoContextFactory;
        private readonly IConfigurationService configurationService;
        private readonly ICloudflareService cloudflareService;
        private readonly IUmbracoFlareUrlService urlService;

        public MediaSavedPurge(IImageCropperService imageCropperService, IUmbracoContextFactory umbracoContextFactory, IConfigurationService configurationService,
            ICloudflareService cloudflareService, IUmbracoFlareUrlService urlService)
        {
            this.imageCropperService = imageCropperService;
            this.umbracoContextFactory = umbracoContextFactory;
            this.configurationService = configurationService;
            this.cloudflareService = cloudflareService;
            this.urlService = urlService;
        }

        public void Handle(MediaSavedNotification notification)
        {
            var umbracoFlareConfigModel = configurationService.LoadConfigurationFile();
            if (!umbracoFlareConfigModel.PurgeCacheOn) { return; }

            var imageCropSizes = imageCropperService.GetAllCrops().ToList();
            var urls = new List<string>();

            var currentDomain = urlService.GetCurrentDomain();

            using (var umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext())
            {
                var mediaCache = umbracoContextReference.UmbracoContext.Media;

                foreach (var media in notification.SavedEntities)
                {
                    if (!media.HasIdentity || media.GetValue<bool>(ApplicationConstants.UmbracoFlareBackendProperties.CloudflareDisabledOnPublishPropertyAlias)) { continue; }

                    var publishedMedia = mediaCache.GetById(media.Id);

                    if (publishedMedia == null)
                    {
                        notification.Messages.Add(new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching, "We could not find the IPublishedContent version of the media: " + media.Id + " you are trying to save.", EventMessageType.Error));
                        continue;
                    }

                    urls.AddRange(imageCropSizes.Select(x => publishedMedia.GetCropUrl(x.Alias)));
                    urls.Add(publishedMedia.Url());
                }
            }

            var fullUrls = urlService.MakeFullUrlsWithDomain(urls, currentDomain, true);
            var result = cloudflareService.PurgePages(fullUrls);

            notification.Messages.Add(result.Success
                ? new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching,
                    "Successfully purged the cloudflare cache.", EventMessageType.Success)
                : new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching,
                    "We could not purge the Cloudflare cache.", EventMessageType.Warning));
        }
    }
}
