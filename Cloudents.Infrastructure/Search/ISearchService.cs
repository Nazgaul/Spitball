using Microsoft.Azure.Search;

namespace Cloudents.Infrastructure.Search
{
    public interface ISearchService
    {
        ISearchIndexClient GetOldClient(string indexName);
    }

    //Taken from https://github.com/auth0/blog/blob/master/_posts/2017-01-05-azure-search-with-aspnetcore.markdown
}