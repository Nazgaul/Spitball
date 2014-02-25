using NHibernate.Id;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        public const string ItemLikeScope = "ItemLike";
        public const string ItemAnnotationScope = "ItemAnnotation";
        public const string ItemAnnotationReplyScope = "ItemReply";
        SnowMaker.BlobOptimisticDataStore m_DataStorage;
        SnowMaker.UniqueIdGenerator m_Generator;
        public IdGenerator()
        {
            m_DataStorage = new SnowMaker.BlobOptimisticDataStore(Storage.StorageProvider.ZboxCloudStorage, BlobProvider.AzureIdGeneratorContainer.ToLower());
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
