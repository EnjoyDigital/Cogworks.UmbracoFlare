//using Cogworks.UmbracoFlare.Core.Constants;
//using Cogworks.UmbracoFlare.Core.Extensions;
//using Cogworks.UmbracoFlare.Core.Helpers;
//using Cogworks.UmbracoFlare.Core.Services;
//using System.Collections.Generic;
//using System.Linq;
//using Umbraco.Cms.Core.Composing;
//using Umbraco.Cms.Core.Events;
//using Umbraco.Cms.Core.Services;
//using Umbraco.Cms.Core.Web;

//namespace Cogworks.UmbracoFlare.Core.Components
//{
//    public class UmbracoFlareEventsComponent : IComponent
//    {
//        private readonly IImageCropperService _imageCropperService;
//        private readonly IUmbracoContextFactory _umbracoContextFactory;
//        private readonly IConfigurationService _configurationService;
//        private readonly IUmbracoFlareDomainService _umbracoFlareDomainService;
//        private readonly ICloudflareService _cloudflareService;
        
//        public UmbracoFlareEventsComponent(IImageCropperService imageCropperService, IUmbracoContextFactory umbracoContextFactory,
//            IConfigurationService configurationService, IUmbracoFlareDomainService umbracoFlareDomainService, ICloudflareService cloudflareService)
//        {
//            _imageCropperService = imageCropperService;
//            _umbracoContextFactory = umbracoContextFactory;
//            _configurationService = configurationService;
//            _umbracoFlareDomainService = umbracoFlareDomainService;
//            _cloudflareService = cloudflareService;
//        }

//        public void Initialize()
//        {
//            ContentService.Published += PurgeCloudflareCache;
//            FileService.SavedScript += PurgeCloudflareCacheForScripts;
//            FileService.SavedStylesheet += PurgeCloudflareCacheForStylesheets;
//            MediaService.Saved += PurgeCloudflareCacheForMedia;

//            TreeControllerBase.MenuRendering += AddPurgeCacheForContentMenu;
//        }

//        private void PurgeCloudflareCache(IContentService sender, ContentPublishedEventArgs e)
//        {
         
//        }

//        public void Terminate()
//        {
//        }

//        private void PurgeCloudflareCacheForScripts(IFileService sender, SaveEventArgs<Script> e)
//        {
//            var files = e.SavedEntities.Select(script => script as File);
//            PurgeCloudflareCacheForFiles(files, e);
//        }

//        private void PurgeCloudflareCacheForStylesheets(IFileService sender, SaveEventArgs<Stylesheet> e)
//        {
//            var files = e.SavedEntities.Select(stylesheet => stylesheet as File);
//            PurgeCloudflareCacheForFiles(files, e);
//        }

//        private void PurgeCloudflareCacheForFiles<T>(IEnumerable<File> files, SaveEventArgs<T> e)
//        {
//            var umbracoFlareConfigModel = _configurationService.LoadConfigurationFile();
//            if (!umbracoFlareConfigModel.PurgeCacheOn) { return; }
//            if (!files.HasAny()) { return; }

//            var currentDomain = UrlService.GetCurrentDomain();
//            var urls = new List<string>();

//            foreach (var file in files)
//            {
//                if (file.IsNew()) { continue; }
//                urls.Add(file.VirtualPath);
//            }

//            var fullUrls = UrlService.MakeFullUrlsWithDomain(urls, currentDomain, true);

//            var result = _cloudflareService.PurgePages(fullUrls);

//            e.Messages.Add(result.Success
//                ? new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching, "Successfully purged the cloudflare cache.",
//                    EventMessageType.Success)
//                : new EventMessage(ApplicationConstants.EventMessageCategory.CloudflareCaching, "We could not purge the Cloudflare cache.",
//                    EventMessageType.Warning));
//        }

//        protected void PurgeCloudflareCacheForMedia(IMediaService sender, SaveEventArgs<IMedia> e)
//        {
         
//        }

//        private static void AddPurgeCacheForContentMenu(TreeControllerBase sender, MenuRenderingEventArgs e)
//        {
//            if (sender.TreeAlias != "content") { return; }

//            var menuItem = new MenuItem("purgeCache", "Purge Cloudflare Cache")
//            {
//                Icon = "umbracoflare-tiny"
//            };

//            menuItem.LaunchDialogView("/App_Plugins/UmbracoFlare/dashboard/views/cogworks.umbracoflare.menu.html", "Purge Cloudflare Cache");
            
//            e.Menu.Items.Insert(e.Menu.Items.Count - 1, menuItem);
//        }
//    }
//}