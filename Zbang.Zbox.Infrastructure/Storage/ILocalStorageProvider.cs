﻿using System.IO;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface ILocalStorageProvider
    {
        /// <summary>
        /// Save file to local storage
        /// </summary>
        /// <param name="streamArray"></param>
        /// <param name="fileName"></param>
        /// <returns>The path to that file</returns>
        Task<string> SaveFileToStorageAsync(Stream streamArray, string fileName);

        string CombineDirectoryWithFileName(string fileName);
        string LocalStorageLocation { get; }
        void DeleteOldFiles();

    }
}
