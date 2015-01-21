﻿using System.IO;

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
        string SaveFileToStorage(Stream streamArray, string fileName);
        byte[] ReadFileFromStorage(string fileName);


        string LocalStorageLocation { get; }
        void DeleteOldFiles();

        //DateTime GetFileLastModified(string fileName);
    }
}
