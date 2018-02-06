using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Storage
{
    public class TempStorageProvider : ITempStorageProvider
    {
        private readonly long _localResourceSize;
        private readonly ILogger _logger;

        public TempStorageProvider(ILogger logger, LocalStorageData meta)
        {
            _logger = logger;
            LocalStorageLocation = meta.Path;
            Directory.CreateDirectory(meta.Path);
            _localResourceSize = meta.Size * 1024 * 1024;
        }

        public string LocalStorageLocation { get; }

        public async Task<string> SaveFileToStorageAsync(Stream streamArray, string fileName)
        {
            if (streamArray == null) throw new ArgumentNullException(nameof(streamArray));
            var fileNameWithPath = CombineDirectoryWithFileName(fileName);

            if (File.Exists(fileNameWithPath))
            {
                var file = new FileInfo(fileNameWithPath);
                if (streamArray.CanSeek && file.Length == streamArray.Length)
                {
                    return fileNameWithPath;
                }
            }
            try
            {
                using (var stream = File.Open(fileNameWithPath, FileMode.Create))
                {
                    await streamArray.CopyToAsync(stream).ConfigureAwait(false);
                }
                return fileNameWithPath;
            }
            catch (IOException ex)
            {
                DeleteOldFiles();
                _logger.Exception(ex, new Dictionary<string, string> {["service"] = "localStorage" });
            }
            using (var stream = File.Open(fileNameWithPath, FileMode.Create))
            {
                await streamArray.CopyToAsync(stream).ConfigureAwait(false);
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
            while (CalculateInitSize() > _localResourceSize * 0.6)
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
            return from file in files let fileInfo = new FileInfo(file) where fileInfo.LastAccessTimeUtc < oldTimeToDeleteFile select fileInfo;
        }

        private long CalculateInitSize()
        {
            var files = Directory.EnumerateFiles(LocalStorageLocation);
            return (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();
        }
    }
}
