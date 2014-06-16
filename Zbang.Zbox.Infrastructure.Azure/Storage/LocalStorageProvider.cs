using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Azure.Storage
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        private readonly string m_LocalResouceLocation;
        private readonly long m_LocalResourceSize;
        private long m_DirectorySize;

        public LocalStorageProvider()
        {
            m_LocalResouceLocation = StorageProvider.LocalResource.LocalResourcePath;
            m_LocalResourceSize = StorageProvider.LocalResource.LocalResourceSizeInMegaBytes * 1024 * 1024;

            m_DirectorySize = CalculateInitSize();
        }



        public string SaveFileToStorage(Stream streamArray, string fileName)
        {
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);

            if (File.Exists(fileNameWithPath))
            {
                var file = new FileInfo(fileNameWithPath);
                if (file.Length == streamArray.Length)
                {
                    return fileNameWithPath;
                }
            }
            m_DirectorySize += streamArray.Length;

            File.WriteAllBytes(fileNameWithPath, streamArray.ConvertToByteArray());
            if (CheckIsDeleteRquired())
            {
                try
                {
                    DeleteOldFiles();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Problem with delete old file from local storage", ex);
                }
            }
            return fileNameWithPath;
        }

        public byte[] ReadFileFromStorage(string fileName)
        {
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);
            if (!File.Exists(fileNameWithPath)) return null;
            File.SetLastAccessTime(fileNameWithPath, DateTime.UtcNow);
            return File.ReadAllBytes(fileNameWithPath);
        }

        public DateTime GetFileLastModified(string fileName)
        {
            var fileInfo = new FileInfo(CombineDirectoryWithFileName(fileName));
            return fileInfo.LastWriteTime;
        }

        private bool CheckIsDeleteRquired()
        {
            if (m_DirectorySize > m_LocalResourceSize * 0.8)
            {
                return true;
            }
            return false;
        }



        private string CombineDirectoryWithFileName(string fileName)
        {
            return Path.Combine(m_LocalResouceLocation, fileName);
        }

        private void DeleteOldFiles()
        {
            DateTime[] timesOfFileDeleteing = { DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddHours(-12), DateTime.UtcNow.AddHours(-1), DateTime.UtcNow };

            int i = 0;
            while (m_DirectorySize > m_LocalResourceSize * 0.6)
            {
                var oldFiles = GetOldFiles(timesOfFileDeleteing[i++]);
                Parallel.ForEach(oldFiles, s => s.Delete());
                m_DirectorySize = CalculateInitSize();
                if (i > timesOfFileDeleteing.Length - 1)
                {
                    break;
                }
            }

        }

        private IEnumerable<FileInfo> GetOldFiles(DateTime oldTimeToDeleteFile)
        {
            var files = Directory.EnumerateFiles(m_LocalResouceLocation);
            var oldFiles = from file in files let fileInfo = new FileInfo(file) where fileInfo.LastAccessTimeUtc < oldTimeToDeleteFile select fileInfo;
            return oldFiles;
        }

        private long CalculateInitSize()
        {
            var files = Directory.EnumerateFiles(m_LocalResouceLocation);
            return (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();
        }
    }
}
