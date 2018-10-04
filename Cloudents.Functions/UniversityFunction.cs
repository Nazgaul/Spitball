using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Functions.Di;
using Cloudents.Functions.Sync;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Spatial;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.Functions
{
    public static class UniversityFunction
    {
        //[FunctionName("UniversityTimer")]
        //[UsedImplicitly]
        //public static async Task RunAsync([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer,
        //    [Blob("spitball/AzureSearch/university-version.txt",FileAccess.ReadWrite)]
        //    CloudBlockBlob blob,
        //    [Inject] IReadRepositoryAsync<(IEnumerable<UniversitySearchWriteDto> update, IEnumerable<SearchWriteBaseDto> delete, long version), SyncAzureQuery> repository,
        //    [Inject] ISearchServiceWrite<University> searchServiceWrite,
        //    TraceWriter log,
        //    CancellationToken token)
        //{
        //    await SyncFunc.SyncAsync(blob, repository, searchServiceWrite, s => new University
        //    {
        //        Name = s.Name,
        //        Image = s.Image,
        //        Extra = s.Extra,
        //        GeographyPoint = GeographyPoint.Create(s.Latitude, s.Longitude),
        //        Id = s.Id.ToString(),
        //        Prefix = new[] { s.Name, s.Extra }.Where(x => x != null).ToArray()
        //    }, log, token).ConfigureAwait(false);
        //    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        //}

        [FunctionName("UniversitySearchSync")]
        public static async Task RunQuestionSearchAsync([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log,
            CancellationToken token)
        {
            const string instanceId = "UniversitySearchSync";
            var existingInstance = await starter.GetStatusAsync(instanceId);
            var startNewInstanceEnum = new[]
            {
                OrchestrationRuntimeStatus.Canceled,
                OrchestrationRuntimeStatus.Completed,
                OrchestrationRuntimeStatus.Failed,
                OrchestrationRuntimeStatus.Terminated
            };
            if (existingInstance == null || startNewInstanceEnum.Contains(existingInstance.RuntimeStatus))
            {
                var model = new SearchSyncInput(SyncType.University);
                await starter.StartNewAsync("SearchSync", instanceId, model);
            }
        }
    }
}
