﻿
namespace Cloudents.Core.Storage
{
    public interface IStorageContainer
    {
        StorageContainer Container { get; }
    }

    public sealed class StorageContainer
    {

        private StorageContainer(string name)
        {
            Name = name.ToLowerInvariant();
            RelativePath = string.Empty;
        }

        private StorageContainer(string name,string relativePath)
        {
            Name = name.ToLowerInvariant();
            RelativePath = relativePath;
        }

        public string Name { get; }
        public string RelativePath { get; }

        public static readonly StorageContainer CacheContainer = new StorageContainer("zboxcahce");
        public static readonly StorageContainer SpitballContainer = new StorageContainer("spitball");
        public static readonly StorageContainer QuestionsAndAnswers = new StorageContainer("spitball-files");
        public static readonly StorageContainer Documents = new StorageContainer("zboxFiles");
        public static readonly StorageContainer IcoFiles = new StorageContainer("zboxhelp","ico");
    }
}
