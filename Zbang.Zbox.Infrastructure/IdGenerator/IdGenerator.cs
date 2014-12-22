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
        public const string UniversityScope = "University";


        // private readonly SnowMaker.BlobOptimisticDataStore m_DataStorage;
        private readonly UniqueIdGenerator m_Generator;
        public IdGenerator()
        {
            //TODO temp
            var storageConnectionString = ConfigFetcher.Fetch("StorageConnectionString");
            CloudStorageAccount cloudStorageAccount = string.IsNullOrEmpty(storageConnectionString) ? CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.Parse(storageConnectionString);
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

        public static Guid GetGuid(DateTime time)
        {
            return GenerateComb(time);
        }


        private static Guid GenerateComb(DateTime time)
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = time;

            // Get the days and milliseconds which will be used to build the byte string 
            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }


        public Guid GetId()
        {
            return GetGuid();
        }
    }
}
