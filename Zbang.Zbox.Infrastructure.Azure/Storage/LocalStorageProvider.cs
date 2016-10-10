using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Azure.Storage
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        private readonly string m_LocalResourceLocation;
        private readonly long m_LocalResourceSize;
        //private long m_DirectorySize;

        public LocalStorageProvider()
        {
            m_LocalResourceLocation = StorageProvider.LocalResource.LocalResourcePath;
            m_LocalResourceSize = StorageProvider.LocalResource.LocalResourceSizeInMegaBytes * 1024 * 1024;
        }

        public string LocalStorageLocation => m_LocalResourceLocation;


        public string SaveFileToStorage(Stream streamArray, string fileName)
        {
            if (streamArray == null) throw new ArgumentNullException(nameof(streamArray));
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);

            if (File.Exists(fileNameWithPath))
            {
                var file = new FileInfo(fileNameWithPath);
                if (file.Length == streamArray.Length)
                {
                    return fileNameWithPath;
                }
            }
            try
            {
                File.WriteAllBytes(fileNameWithPath, streamArray.ConvertToByteArray());
            }
            catch (IOException ex)
            {
                DeleteOldFiles();
                TraceLog.WriteError("on writing in storage" + ex);
            }
            File.WriteAllBytes(fileNameWithPath, streamArray.ConvertToByteArray());
            return fileNameWithPath;
        }

        public byte[] ReadFileFromStorage(string fileName)
        {
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);
            if (!File.Exists(fileNameWithPath)) return null;
            File.SetLastAccessTime(fileNameWithPath, DateTime.UtcNow);
            return File.ReadAllBytes(fileNameWithPath);
        }


       

        


        private string CombineDirectoryWithFileName(string fileName)
        {
            return Path.Combine(m_LocalResourceLocation, fileName);
        }

        public void DeleteOldFiles()
        {
            DateTime[] timesOfFileDeleting = { DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddHours(-12), DateTime.UtcNow.AddHours(-1), DateTime.UtcNow };

            int i = 0;
            while (CalculateInitSize() > m_LocalResourceSize * 0.6)
            {
                var oldFiles = GetOldFiles(timesOfFileDeleting[i++]);
                Parallel.ForEach(oldFiles, s => s.Delete());
                if (i > timesOfFileDeleting.Length - 1)
                {
                    break;
                }
            }

        }

        private IEnumerable<FileInfo> GetOldFiles(DateTime oldTimeToDeleteFile)
        {
            var files = Directory.EnumerateFiles(m_LocalResourceLocation);
            var oldFiles = from file in files let fileInfo = new FileInfo(file) where fileInfo.LastAccessTimeUtc < oldTimeToDeleteFile select fileInfo;
            return oldFiles;
        }

        private long CalculateInitSize()
        {
            var files = Directory.EnumerateFiles(m_LocalResourceLocation);
            return (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();
        }
    }
}
