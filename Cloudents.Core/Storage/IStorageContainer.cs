using System;

namespace Cloudents.Core.Storage
{
    public interface IStorageContainer
    {
        StorageContainer Container { get; }
        //string RelativePath { get; }
    }

    //public class StorageAttribute : Attribute
    //{
    //    public StorageAttribute(string name, string relativePath)
    //    {
    //        Name = name.ToLowerInvariant();
    //        RelativePath = relativePath;
    //    }

    //    public StorageAttribute(string name)
    //    {
    //        Name = name.ToLowerInvariant();
    //        RelativePath = string.Empty;
    //    }

    //    public string Name { get; }
    //    public string RelativePath { get; }
    //}

    //public enum StorageContainer2
    //{
    //    [Storage("zboxcahce")]
    //    Cache,
    //    [Storage("spitball")]
    //    Spitball,
    //    [Storage("spitball-files")]
    //    QuestionsAndAnswers,
    //    [Storage("zboxFiles")]
    //    Documents
    //}

    public sealed class StorageContainer
    {
        private StorageContainer(string name, string relativePath)
        {
            Name = name.ToLowerInvariant();
            RelativePath = relativePath;
        }

        private StorageContainer(string name)
        {
            Name = name.ToLowerInvariant();
            RelativePath = string.Empty;
        }

        public string Name { get; }
        public string RelativePath { get; }


        public static readonly StorageContainer CacheContainer = new StorageContainer("zboxcahce");
        public static readonly StorageContainer SpitballContainer = new StorageContainer("spitball");
        public static readonly StorageContainer QuestionsAndAnswers = new StorageContainer("spitball-files");
        public static readonly StorageContainer Documents = new StorageContainer("zboxFiles");

        
    }
}
