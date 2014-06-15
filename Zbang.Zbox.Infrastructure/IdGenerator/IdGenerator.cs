using Microsoft.WindowsAzure.Storage;
using NHibernate.Id;
using System;
using System.Configuration;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        public const string ItemLikeScope = "ItemLike";
        public const string ItemAnnotationScope = "ItemAnnotation";
        public const string ItemAnnotationReplyScope = "ItemReply";

        public const string QuizScope = "Quiz";
        // private readonly SnowMaker.BlobOptimisticDataStore m_DataStorage;
        private readonly SnowMaker.UniqueIdGenerator m_Generator;
        public IdGenerator()
        {
            //TODO temp
            CloudStorageAccount cloudStorageAccount;
            try
            {
                cloudStorageAccount = CloudStorageAccount.Parse(ConfigFetcher.Fetch("StorageConnectionString"));
            }
            catch (ConfigurationErrorsException)
            {
                cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            var dataStorage = new SnowMaker.BlobOptimisticDataStore(
                 cloudStorageAccount,
                //BlobProvider.AzureIdGeneratorContainer.ToLower()
                 "zboxIdGenerator"
                 );
            m_Generator = new SnowMaker.UniqueIdGenerator(dataStorage) { BatchSize = 20 };
        }

        public long GetId(string scopeName)
        {
            return m_Generator.NextId(scopeName);

        }





        public static Guid GetGuid()
        {
            var combGenerator = new GuidCombGenerator();
            return (Guid)combGenerator.Generate(null, null);
        }


        public Guid GetId()
        {
            return GetGuid();
        }
    }
}
