
using System.Collections.Generic;

namespace Cloudents.Web.Seo
{
    public interface IBuildSeo
    {
        IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index);


    }
}