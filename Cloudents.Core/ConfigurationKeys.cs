﻿using Cloudents.Core.Interfaces;

namespace Cloudents.Core
{
    public class ConfigurationKeys : IConfigurationKeys
    {
        public string Db { get; set; }
        public SearchServiceCredentials Search { get; set; }
        public string Redis { get; set; }
        public string Storage { get; set; }
        public string SystemUrl { get; set; }

        public LocalStorageData LocalStorageData { get; set; }
    }

    public class SearchServiceCredentials
    {
        public SearchServiceCredentials(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }
        public string Key { get; }
    }

    public class LocalStorageData
    {
        public LocalStorageData(string path, int size)
        {
            Path = path;
            Size = size;
        }

        public string Path { get; }
        public int Size { get; }
    }
}
