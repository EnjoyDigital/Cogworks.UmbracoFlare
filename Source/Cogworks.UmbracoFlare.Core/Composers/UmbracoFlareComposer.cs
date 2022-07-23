using Cogworks.UmbracoFlare.Core.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Cogworks.UmbracoFlare.Core.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Cogworks.UmbracoFlare.Core.Helpers;

namespace Cogworks.UmbracoFlare.Core.Composers
{
    public class UmbracoFlareComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentPublishingNotification, ContentPublishingPurge>();
            builder.Services.AddSingleton<IUmbracoFlareDomainService, UmbracoFlareDomainService>();
            builder.Services.AddSingleton<IUmbracoFlareUrlService, UmbracoFlareUrlService>();
        }
    }
}