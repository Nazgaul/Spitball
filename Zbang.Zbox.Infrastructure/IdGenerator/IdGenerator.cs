using Microsoft.WindowsAzure.Storage;
using NHibernate.Id;
using System;
using SnowMaker;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        public const string ItemLikeScope = "ItemLike";
        public const string ItemAnnotationScope = "ItemAnnotation";
        public const string ItemAnnotationReplyScope = "ItemReply";
        public const string QuizScope = "Quiz";
        public const string DepartmentScope = "Department";


        // private readonly SnowMaker.BlobOptimisticDataStore m_DataStorage;
        private readonly UniqueIdGenerator m_Generator;
        public IdGenerator()
        {
            //TODO temp
            CloudStorageAccount cloudStorageAccount;
            var storageConnectionString = ConfigFetcher.Fetch("StorageConnectionString");
            cloudStorageAccount = string.IsNullOrEmpty(storageConnectionString) ? CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.Parse(storageConnectionString);
            var dataStorage = new BlobOptimisticDataStore(
                 cloudStorageAccount,
                 "zboxIdGenerator"
                 );
            m_Generator = new UniqueIdGenerator(dataStorage) { BatchSize = 20 };
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
