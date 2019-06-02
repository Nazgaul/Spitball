using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Query;
using Cloudents.Query.SearchSync;
using Cloudents.Search.Entities;
using Cloudents.Search.Tutor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class TutorFunction
    {
        [FunctionName("TutorFunction")]
        public static async Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Blob("spitball/AzureSearch/tutor-version.txt")] CloudBlockBlob blob,
            [AzureSearchSync(TutorSearchWrite.IndexName)] IAsyncCollector<AzureSearchSyncOutput> indexInstance,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            var query = new TutorSyncAzureSearchQuery(0, null);
            if (await blob.ExistsAsync())
            {
                var str = await blob.DownloadTextAsync();
                query = JsonConvert.DeserializeObject<TutorSyncAzureSearchQuery>(str);
            }
            var result = await queryBus.QueryAsync(query, token);
            
            foreach (var update in result.Update)
            {

                await indexInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Tutor()
                    {
                        Country = update.Country.ToUpperInvariant(),
                        Id = update.Id.ToString(),
                        Name = update.Name,
                        Price = update.Price,
                        Courses = update.Courses.ToArray(),
                        Image = update.Image,
                        Bio = update.Bio,
                        Rate = update.Rate,
                        InsertDate = DateTime.UtcNow,
                        Prefix = update.Courses.ToArray(),
                        ReviewCount = update.ReviewsCount,
                        Subjects = update.Subjects.ToArray()
                    },
                    Insert = true

                }, token);
            }

            foreach (var delete in result.Delete)
            {
                await indexInstance.AddAsync(new AzureSearchSyncOutput()
                {
                    Item = new Tutor()
                    {
                        Id = delete
                    },
                    Insert = false

                }, token);

            }

            var nextVersion = new TutorSyncAzureSearchQuery(result.Version, result.Update.OrderByDescending(o=>o.VersionAsLong).First().Version);
            var jsonStr = JsonConvert.SerializeObject(nextVersion);
            await blob.UploadTextAsync(jsonStr);
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
