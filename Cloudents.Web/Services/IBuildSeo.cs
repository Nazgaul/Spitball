
using System.Collections.Generic;

namespace Cloudents.Web.Services
{
    public interface IBuildSeo
    {
        IEnumerable<string> GetUrls(int index);


    }
}