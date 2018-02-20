using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class UniversityFunction
    {
        [FunctionName("UniversityTimer")]
        public static async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Read)]  string blobRead,
            [Blob("spitball/AzureSearch/university-version.txt", FileAccess.Write)] TextWriter blobWrite,
            [Inject] IReadRepositoryAsync<(List<UniversityWriteDto> update, IEnumerable<long> delete, long version), long> repository,
            TraceWriter log,
            CancellationToken token)
        {
            // blob.ReadFromStreamAsync()
            var version = long.Parse(blobRead);
            // int.Parse(await blob.DownloadTextAsync(token));
            var data = await repository.GetAsync(version, token).ConfigureAwait(false);
            await blobWrite.WriteAsync(data.version.ToString()).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        public static void ProcessUpload()
        {

        }

        public static void ProcessDelete()
        {

        }

        [FunctionName("SynonymWatch")]
        public static async Task SynonymWatch([BlobTrigger("spitball/AzureSearch/{name}")]
            string content, string name, TraceWriter log,
            [Inject] ISynonymWrite synonymWrite,
            CancellationToken token)
        {
            var fileName = Path.GetFileNameWithoutExtension(name);
            if (string.Equals(fileName, UniversitySearchWrite.SynonymName, StringComparison.InvariantCultureIgnoreCase))
            {
                await synonymWrite.CreateOrUpdateAsync(fileName, content, token).ConfigureAwait(false);
                log.Info(content);
            }
        }
    }
}
