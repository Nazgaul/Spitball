
using System.Collections.Generic;

namespace Cloudents.Web.Seo
{
    public interface IBuildSeo
    {
        IEnumerable<SitemapNode> GetUrls(int index);


    }
}