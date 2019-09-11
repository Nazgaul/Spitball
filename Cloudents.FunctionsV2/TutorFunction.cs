using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.FunctionsV2.Binders;
using Cloudents.Query;
using Cloudents.Query.SearchSync;
using Cloudents.Search.Entities;
using Cloudents.Search.Tutor;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class TutorFunction
    {
        [FunctionName("TutorFunction")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
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


            var nextQuery = new TutorSyncAzureSearchQuery(query.Version, query.RowVersion);

            bool updateOccur;
            do
            {
                updateOccur = false;
                var result = await queryBus.QueryAsync(query, token);

                foreach (var update in result.Update)
                {
                    log.LogInformation($"Sync {update}");
                    updateOccur = true;
                    var courses = update.Courses?.Where(w => !string.IsNullOrWhiteSpace(w)).Distinct().ToArray() ??
                                  new string[0];
                    var subjects = update.Subjects?.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray() ?? new string[0];
                    await indexInstance.AddAsync(new AzureSearchSyncOutput()
                    {
                        Item = new Tutor
                        {
                            Country = update.Country.ToUpperInvariant(),
                            Id = update.UserId.ToString(),
                            Name = update.Name,
                            Courses = courses.ToArray(),
                            Rate = update.Rate,

                            InsertDate = DateTime.UtcNow,
                            Prefix = courses.Union(subjects).Union(new[] { update.Name })
                            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                            ReviewCount = update.ReviewsCount,
                            Subjects = subjects.ToArray(),
                            Data = new TutorCardDto()
                            {
                                UserId = update.UserId,
                                Name = update.Name,
                                Courses = courses.Take(3),
                                Subjects = subjects.OrderBy(o => o).Take(3),
                                ReviewsCount = update.ReviewsCount,
                                Rate = (float)update.Rate,
                                University = update.University,
                                Lessons = Math.Max(update.LessonsCount, update.ReviewsCount),
                                Bio = update.Bio,
                                Price = (int)update.Price,
                                Image = update.Image,
                            }
                        },
                        Insert = true

                    }, token);
                }

                foreach (var delete in result.Delete)
                {
                    updateOccur = true;
                    await indexInstance.AddAsync(new AzureSearchSyncOutput()
                    {
                        Item = new Tutor()
                        {
                            Id = delete
                        },
                        Insert = false

                    }, token);
                }

                query.Page++;
                var versionElement = result.Update.OrderByDescending(o => o.VersionAsLong).FirstOrDefault();
                if (versionElement != null)
                {
                    nextQuery.Version = Math.Max(nextQuery.Version, result.Version);
                    nextQuery.RowVersion = versionElement.Version;
                }

                await indexInstance.FlushAsync(token);
              
            } while (updateOccur);

            if (query.Page > 0)
            {
                var jsonStr = JsonConvert.SerializeObject(nextQuery);
                await blob.UploadTextAsync(jsonStr);
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
