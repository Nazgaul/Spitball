using Microsoft.WindowsAzure.Storage;
using SnowMaker;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class IdGenerator : IIdGenerator
    {
        private readonly UniqueIdGenerator m_Generator;
        public IdGenerator()
        {
            //TODO temp
            var storageConnectionString = ConfigFetcher.Fetch("StorageConnectionString");
            var cloudStorageAccount = string.IsNullOrEmpty(storageConnectionString) ? CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.Parse(storageConnectionString);
            var dataStorage = new BlobOptimisticDataStore(
                 cloudStorageAccount,
                 "zboxIdGenerator"
                 );
            m_Generator = new UniqueIdGenerator(dataStorage) { BatchSize = 5 };
        }

        public long GetId(string scopeName)
        {
            return m_Generator.NextId(scopeName);

        }
    }
}
