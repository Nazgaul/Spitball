using Microsoft.WindowsAzure.Storage;
using NHibernate.Id;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        public const string ItemLikeScope = "ItemLike";
        public const string ItemAnnotationScope = "ItemAnnotation";
        public const string ItemAnnotationReplyScope = "ItemReply";

        public const string QuizScope = "Quiz";
        SnowMaker.BlobOptimisticDataStore m_DataStorage;
        SnowMaker.UniqueIdGenerator m_Generator;
        public IdGenerator(IBlobProvider blobProvider)
        {
            //TODO temp
            CloudStorageAccount _cloudStorageAccount;
            try
            {
                _cloudStorageAccount = CloudStorageAccount.Parse(ConfigFetcher.Fetch("StorageConnectionString"));
            }
            catch (ConfigurationErrorsException)
            {
                _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            m_DataStorage = new SnowMaker.BlobOptimisticDataStore(
                _cloudStorageAccount,
                //BlobProvider.AzureIdGeneratorContainer.ToLower()
                "zboxIdGenerator"
                );
            m_Generator = new SnowMaker.UniqueIdGenerator(m_DataStorage) { BatchSize = 20 };
        }

        public long GetId(string scopeName)
        {
            return m_Generator.NextId(scopeName);

        }





        public static Guid GetGuid()
        {
           GuidCombGenerator CombGenerator = new GuidCombGenerator();
           return (Guid)CombGenerator.Generate(null, null);
        }


        public Guid GetId()
        {
            return IdGenerator.GetGuid();
        }
    }
}
