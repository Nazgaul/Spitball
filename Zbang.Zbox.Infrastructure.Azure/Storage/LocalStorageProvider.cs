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
        private readonly long m_LocalResourceSize;

        private readonly ILogger m_Logger;
        //private long m_DirectorySize;

        public LocalStorageProvider(ILogger logger)
        {
            m_Logger = logger;
            LocalStorageLocation = StorageProvider.LocalResource.LocalResourcePath;
            m_LocalResourceSize = StorageProvider.LocalResource.LocalResourceSizeInMegaBytes * 1024 * 1024;
        }

        public string LocalStorageLocation { get; }

        public async Task<string> SaveFileToStorageAsync(Stream streamSource, string fileName)
        {
            if (streamSource == null) throw new ArgumentNullException(nameof(streamSource));
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);

            if (File.Exists(fileNameWithPath))
            {
                var file = new FileInfo(fileNameWithPath);
                if (streamSource.CanSeek && file.Length == streamSource.Length)
                {
                    return fileNameWithPath;
                }
            }
            try
            {
                using (var stream = File.Open(fileNameWithPath, FileMode.Create))
                {
                    await streamSource.CopyToAsync(stream).ConfigureAwait(false);
                }
                return fileNameWithPath;
            }
            catch (IOException ex)
            {
                DeleteOldFiles();
                m_Logger.Exception(ex, new Dictionary<string, string> {["service"] = "localStorage" });
            }
            using (var stream = File.Open(fileNameWithPath, FileMode.Create))
            {
                await streamSource.CopyToAsync(stream).ConfigureAwait(false);
            }
            return fileNameWithPath;
        }

        public string CombineDirectoryWithFileName(string fileName)
        {
            return Path.Combine(LocalStorageLocation, fileName);
        }

        public void DeleteOldFiles()
        {
            DateTime[] timesOfFileDeleting = { DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddHours(-12), DateTime.UtcNow.AddHours(-1), DateTime.UtcNow };

            var i = 0;
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
            var files = Directory.EnumerateFiles(LocalStorageLocation);
            var oldFiles = from file in files let fileInfo = new FileInfo(file) where fileInfo.LastAccessTimeUtc < oldTimeToDeleteFile select fileInfo;
            return oldFiles;
        }

        private long CalculateInitSize()
        {
            var files = Directory.EnumerateFiles(LocalStorageLocation);
            return (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();
        }
    }
}
