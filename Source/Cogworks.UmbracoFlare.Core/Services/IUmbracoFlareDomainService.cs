using Cogworks.UmbracoFlare.Core.Models.Cloudflare;
using System.Collections.Generic;

namespace Cogworks.UmbracoFlare.Core.Services
{
    public interface IUmbracoFlareDomainService
    {
        IEnumerable<string> GetUrlsForNode(int contentId, string currentDomain, bool includeDescendants = false);

        IEnumerable<string> GetAllowedCloudflareDomains();

        IEnumerable<string> GetAllUrlsForWildCardUrls(IEnumerable<string> wildCardUrls);

        Zone GetZoneFilteredByDomain(string domainUrl);
    }
}