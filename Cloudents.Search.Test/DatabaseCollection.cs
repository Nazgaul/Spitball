using Xunit;

namespace Cloudents.Search.Test
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<SearchFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
