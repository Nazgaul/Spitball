using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [CollectionDefinition("Database collection2")]
    public class DatabaseCollection2 : ICollectionFixture<DatabaseFixture2>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}