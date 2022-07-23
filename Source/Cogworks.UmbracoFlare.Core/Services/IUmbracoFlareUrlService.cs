using System.Collections.Generic;

namespace Cogworks.UmbracoFlare.Core.Services
{
    public interface IUmbracoFlareUrlService
    {
        string GetCurrentDomain();
        IEnumerable<string> GetFullUrlForPurgeStaticFiles(string url, string currentDomain, bool withScheme);
        IEnumerable<string> MakeFullUrlsWithDomain(IEnumerable<string> urls, string currentDomain, bool withScheme);
        string MakeFullUrlWithDomain(string url, string domain, bool withScheme);
    }
}